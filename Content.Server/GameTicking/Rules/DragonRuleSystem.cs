using Content.Server.Antag;
using Content.Server.Dragon;
using Content.Server.GameTicking.Rules.Components;
using Content.Server.Mind;
using Content.Server.Roles;
using Content.Server.Station.Components;
using Content.Server.Station.Systems;
using Content.Shared.CharacterInfo;
using Content.Shared.Devour.Components;
using Content.Shared.Localizations;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Prometheus;
using Robust.Server.GameObjects;

namespace Content.Server.GameTicking.Rules;

public sealed class DragonRuleSystem : GameRuleSystem<DragonRuleComponent>
{
    #region Starlight
    private static readonly Histogram _dragonWinInfo = Metrics.CreateHistogram(
        "sl_dragon_winning",
        "Contains info on if a dragon won and if so how hard they won",
        ["alive", "rifts", "eaten"]
    );
    #endregion

    [Dependency] private readonly TransformSystem _transform = default!;
    [Dependency] private readonly AntagSelectionSystem _antag = default!;
    [Dependency] private readonly StationSystem _station = default!;
    [Dependency] private readonly RoleSystem _roleSystem = default!;
    [Dependency] private readonly MindSystem _mind = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DragonRuleComponent, AfterAntagEntitySelectedEvent>(AfterAntagEntitySelected);
        SubscribeLocalEvent<DragonRoleComponent, GetBriefingEvent>(UpdateBriefing);
    }

    private void UpdateBriefing(Entity<DragonRoleComponent> entity, ref GetBriefingEvent args)
    {
        var ent = args.Mind.Comp.OwnedEntity;

        if (ent is null)
            return;

        args.Append(MakeBriefing(ent.Value));
    }

    private void AfterAntagEntitySelected(Entity<DragonRuleComponent> ent, ref AfterAntagEntitySelectedEvent args)
    {
        if (!_mind.TryGetMind(args.EntityUid, out var mindId, out var mind))
            return;

        _roleSystem.MindHasRole<DragonRoleComponent>(mindId, out var dragonRole);

        if (dragonRole is null)
            return;

        _antag.SendBriefing(args.EntityUid, MakeBriefing(args.EntityUid), null, null);
    }

    private string MakeBriefing(EntityUid dragon)
    {
        var direction = string.Empty;

        var dragonXform = Transform(dragon);

        var station = _station.GetStationInMap(dragonXform.MapID);
        EntityUid? stationGrid = null;
        if (TryComp<StationDataComponent>(station, out var stationData))
            stationGrid = _station.GetLargestGrid(stationData);

        if (stationGrid is not null)
        {
            var stationPosition = _transform.GetWorldPosition((EntityUid)stationGrid);
            var dragonPosition = _transform.GetWorldPosition(dragon);

            var vectorToStation = stationPosition - dragonPosition;
            direction = ContentLocalizationManager.FormatDirection(vectorToStation.GetDir());
        }

        var briefing = Loc.GetString("dragon-role-briefing", ("direction", direction));

        return briefing;
    }

    #region Starlight data collection
    private void OnRoundEnded(RoundEndTextAppendEvent _)
    {
        var query = EntityManager.AllEntities<DragonComponent>();
        foreach (var dragon in query)
        {
            var devoured = 0;
            if (TryComp<DevourerComponent>(dragon.Owner, out var devour))
                devoured = devour.Devoured;
            var alive = false;
            if (TryComp<MobStateComponent>(dragon.Owner, out var state))
                alive = state.CurrentState == MobState.Alive;
            _dragonWinInfo.WithLabels([
                alive.ToString(),
                dragon.Comp.Rifts?.ToString() ?? "0",
                devoured.ToString()
            ]);  
        }
    }
    #endregion
}
