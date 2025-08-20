using Content.Shared.Starlight.Utility;
using Content.Shared.Starlight;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System.Numerics;
using Content.Shared._NullLink;

namespace Content.Shared.Starlight.GhostTheme;

[Prototype("ghostTheme")]
public sealed class GhostThemePrototype : IPrototype
{
    [IdDataField]
    public string ID { get; } = default!;
    
    [DataField("name")]
    public string Name { get; private set; } = string.Empty;
    
    [DataField("description")]
    public string Description { get; private set; } = string.Empty;
    
    [DataField("spriteSpecifier", required: true)]
    public ExtendedSpriteSpecifier SpriteSpecifier { get; private set; } = default!;
    
    [DataField("requirement")]
    public ProtoId<RoleRequirementPrototype>? Requirement;
    
    [DataField("requiredCkey")]
    public string? Ckey = null;
    
    [DataField("colorizeable")]
    public bool Colorizeable = false;
}