using Content.Shared.Movement.Systems;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared._Starlight.Actions.Jump;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class JumpComponent : Component
{
    [DataField, AutoNetworkedField]
    public EntProtoId Action = "Jump";

    [DataField, AutoNetworkedField]
    public EntityUid? ActionEntity;

    [DataField, AutoNetworkedField]
    public bool IsEquipment = false;
}