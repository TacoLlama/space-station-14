using Content.Shared.Access.Systems;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Roles;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Access.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(SharedIdCardConsoleSystem))]
public sealed partial class IdCardConsoleComponent : Component
{
    public static string PrivilegedIdCardSlotId = "IdCardConsole-privilegedId";
    public static string TargetIdCardSlotId = "IdCardConsole-targetId";

    [DataField]
    public ItemSlot PrivilegedIdSlot = new();

    [DataField]
    public ItemSlot TargetIdSlot = new();

    [Serializable, NetSerializable]
    public sealed class WriteToTargetIdMessage : BoundUserInterfaceMessage
    {
        public readonly string FullName;
        public readonly string JobTitle;
        public readonly List<ProtoId<AccessLevelPrototype>> AccessList;
        public readonly ProtoId<JobPrototype> JobPrototype;

        public WriteToTargetIdMessage(string fullName, string jobTitle, List<ProtoId<AccessLevelPrototype>> accessList, ProtoId<JobPrototype> jobPrototype)
        {
            FullName = fullName;
            JobTitle = jobTitle;
            AccessList = accessList;
            JobPrototype = jobPrototype;
        }
    }
    // Starlight-edit: Start
    [Serializable, NetSerializable]
    public sealed class AccessGroupSelectedMessage : BoundUserInterfaceMessage
    {
        public readonly ProtoId<AccessGroupPrototype> SelectedGroup;

        public AccessGroupSelectedMessage(ProtoId<AccessGroupPrototype> selectedGroup)
        {
            SelectedGroup = selectedGroup;
        }
    }
    // Starlight-edit: End

    // Put this on shared so we just send the state once in PVS range rather than every time the UI updates.

    [DataField, AutoNetworkedField]
    // Starlight-edit: Start
    public List<ProtoId<AccessGroupPrototype>> AccessGroups = new();
    [AutoNetworkedField]
    public ProtoId<AccessGroupPrototype>? CurrentAccessGroup;
    // Starlight-edit: End

    [Serializable, NetSerializable]
    public sealed class IdCardConsoleBoundUserInterfaceState : BoundUserInterfaceState
    {
        public readonly string PrivilegedIdName;
        public readonly bool IsPrivilegedIdPresent;
        public readonly bool IsPrivilegedIdAuthorized;
        public readonly bool IsTargetIdPresent;
        public readonly string TargetIdName;
        public readonly string? TargetIdFullName;
        public readonly string? TargetIdJobTitle;
        public readonly List<ProtoId<AccessLevelPrototype>>? TargetIdAccessList;
        public readonly List<ProtoId<AccessLevelPrototype>>? AllowedModifyAccessList;
        public readonly ProtoId<JobPrototype> TargetIdJobPrototype;
        // Starlight-edit: Start
        public readonly ProtoId<AccessGroupPrototype> CurrentAccessGroup;
        public readonly List<ProtoId<AccessGroupPrototype>>? AvailableAccessGroups;
        // Starlight-edit: End

        public IdCardConsoleBoundUserInterfaceState(bool isPrivilegedIdPresent,
            bool isPrivilegedIdAuthorized,
            bool isTargetIdPresent,
            string? targetIdFullName,
            string? targetIdJobTitle,
            List<ProtoId<AccessLevelPrototype>>? targetIdAccessList,
            List<ProtoId<AccessLevelPrototype>>? allowedModifyAccessList,
            ProtoId<JobPrototype> targetIdJobPrototype,
            string privilegedIdName,
            string targetIdName,
            // Starlight-edit: Start
            ProtoId<AccessGroupPrototype> currentAccessGroup,
            List<ProtoId<AccessGroupPrototype>>? availableAccessGroups = null)
            // Starlight-edit: End
        {
            IsPrivilegedIdPresent = isPrivilegedIdPresent;
            IsPrivilegedIdAuthorized = isPrivilegedIdAuthorized;
            IsTargetIdPresent = isTargetIdPresent;
            TargetIdFullName = targetIdFullName;
            TargetIdJobTitle = targetIdJobTitle;
            TargetIdAccessList = targetIdAccessList;
            AllowedModifyAccessList = allowedModifyAccessList;
            TargetIdJobPrototype = targetIdJobPrototype;
            PrivilegedIdName = privilegedIdName;
            TargetIdName = targetIdName;
            // Starlight-edit: Start
            CurrentAccessGroup = currentAccessGroup;
            AvailableAccessGroups = availableAccessGroups; 
            // Starlight-edit: End
        }
    }

    [Serializable, NetSerializable]
    public enum IdCardConsoleUiKey : byte
    {
        Key,
    }
}
