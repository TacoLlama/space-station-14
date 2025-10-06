using Content.Shared.Access.Systems;
using Content.Shared.CriminalRecords.Components;
using Content.Shared.CriminalRecords;
using Content.Shared.StationRecords;
using Content.Shared._CD.Records;
using JetBrains.Annotations;
using Robust.Client.Player;

namespace Content.Client._CD.Records.UI;

[UsedImplicitly]
public sealed class CharacterRecordConsoleBoundUserInterface : BoundUserInterface
{
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    private readonly AccessReaderSystem _accessReader;

    [ViewVariables] private CharacterRecordViewer? _window;

    public CharacterRecordConsoleBoundUserInterface(EntityUid owner, Enum key) : base(owner, key)
    {
        _accessReader = EntMan.System<AccessReaderSystem>();
    }

    protected override void UpdateState(BoundUserInterfaceState baseState)
    {
        base.UpdateState(baseState);
        if (baseState is not CharacterRecordConsoleState state)
            return;

        if (_window?.IsSecurity() ?? false)
        {
            var comp = EntMan.GetComponent<CriminalRecordsConsoleComponent>(Owner);
            _window.SecurityWantedStatusMaxLength = comp.MaxStringLength;

            var hasAccess = false;
            var session = _playerManager.LocalSession;
            if (session?.AttachedEntity is { } attached)
                hasAccess = _accessReader.IsAllowed(attached, Owner);

            _window.SetSecurityHistoryEditable(hasAccess);
        }
        else
        {
            _window?.SetSecurityHistoryEditable(false);
        }

        _window?.UpdateState(state);
    }

    protected override void Open()
    {
        base.Open();

        _window = new();
        _window.OnClose += Close;
        _window.OnListingItemSelected += meta =>
        {
            SendMessage(new CharacterRecordConsoleSelectMsg(meta?.CharacterRecordKey));

            // If we are a security records console, we also need to inform the criminal records
            // system of our state.
            if (_window.IsSecurity() && meta?.StationRecordKey != null)
            {
                SendMessage(new SelectStationRecord(meta.Value.StationRecordKey.Value));
                _window.SetSecurityStatusEnabled(true);
            }
            else
            {
                // If the user does not have criminal records for some reason, we should not be able
                // to set their wanted status
                _window.SetSecurityStatusEnabled(false);
            }
        };

        _window.OnFiltersChanged += (ty, txt) =>
        {
            SendMessage(txt == null
                ? new CharacterRecordsConsoleFilterMsg(null)
                : new CharacterRecordsConsoleFilterMsg(new StationRecordsFilter(ty, txt)));
        };

        _window.OnSetSecurityStatus += (status, reason) => SendMessage(new CriminalRecordChangeStatus(status, reason));
        _window.OnAddSecurityHistory += line => SendMessage(new CriminalRecordAddHistory(line));
        _window.OnDeleteSecurityHistory += index => SendMessage(new CriminalRecordDeleteHistory((uint) index));

        _window.OpenCentered();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        _window?.Close();
    }
}
