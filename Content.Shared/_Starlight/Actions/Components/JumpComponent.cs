using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Starlight.Actions.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class JumpComponent : Component
{
    [DataField, AutoNetworkedField]
    public EntProtoId Action = "Jump";

    [DataField, AutoNetworkedField]
    public EntityUid? ActionEntity;

    [DataField, AutoNetworkedField]
    public bool IsEquipment = false;

    [DataField]
    public bool KnockdownTargetOnCollision = false;

    [DataField]
    public float KnockdownTargetDuration = 1f;

    [DataField]
    public bool KnockdownSelfOnCollision = false;

    [DataField]
    public float KnockdownSelfDuration = 1f;

    [DataField]
    public float Cooldown = 30f;
}