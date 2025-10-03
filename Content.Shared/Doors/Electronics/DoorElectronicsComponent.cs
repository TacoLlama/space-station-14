using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Prototypes;
using Content.Shared.Access;

namespace Content.Shared.Doors.Electronics;

/// <summary>
/// Allows an entity's AccessReader to be configured via UI.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState] // Starlight edit
public sealed partial class DoorElectronicsComponent : Component
{
    // Starlight Start
    [DataField, AutoNetworkedField]
    public List<ProtoId<AccessGroupPrototype>> AccessGroups = new();
    // Starlight End
}

[Serializable, NetSerializable]
public sealed class DoorElectronicsUpdateConfigurationMessage : BoundUserInterfaceMessage
{
    public List<ProtoId<AccessLevelPrototype>> AccessList;

    public DoorElectronicsUpdateConfigurationMessage(List<ProtoId<AccessLevelPrototype>> accessList)
    {
        AccessList = accessList;
    }
}

[Serializable, NetSerializable]
public sealed class DoorElectronicsConfigurationState : BoundUserInterfaceState
{
    public List<ProtoId<AccessLevelPrototype>> AccessList;
    public List<ProtoId<AccessGroupPrototype>> AccessGroups; // Starlight

    public DoorElectronicsConfigurationState(List<ProtoId<AccessLevelPrototype>> accessList, List<ProtoId<AccessGroupPrototype>> accessGroups) // Starlight edit
    {
        AccessList = accessList;
        AccessGroups = accessGroups; // Starlight
    }
}

[Serializable, NetSerializable]
public enum DoorElectronicsConfigurationUiKey : byte
{
    Key
}
