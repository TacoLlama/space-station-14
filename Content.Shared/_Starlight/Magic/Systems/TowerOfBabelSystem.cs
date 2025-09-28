using System.Linq;
using Content.Shared._Starlight.Language;
using Content.Shared._Starlight.Language.Components;
using Content.Shared._Starlight.Language.Events;
using Content.Shared._Starlight.Language.Systems;
using Content.Shared._Starlight.Magic.Components;
using Content.Shared.Destructible;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared._Starlight.Magic.Systems;

public abstract partial class TowerOfBabelSystem : EntitySystem
{
    [Dependency] private readonly SharedLanguageSystem _language = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<TowerOfBabelComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<TowerOfBabelComponent, DestructionEventArgs>(OnDestruction);
        SubscribeLocalEvent<LanguageKnowledgeInitEvent>(OnLanguageKnowledgeInit);
    }

    private void ShuffleLanguages(Entity<LanguageKnowledgeComponent> languageKnower, List<ProtoId<LanguagePrototype>>? allLangs = null)
    {
        allLangs ??= _language.Languages.ToList(); //we can skip it if we know we wont be re-using it for perf reasons.
        _language.CaptureCache(languageKnower);

        var comp = languageKnower.Comp;
        if (HasComp<UniversalLanguageSpeakerComponent>(languageKnower))
            return; // One who knows the knowledge of all things cannot know less.

        if (comp.SpokenLanguages.Count > comp.UnderstoodLanguages.Count)
        {
            _random.Shuffle(allLangs);
            comp.SpokenLanguages = [.. allLangs.Take(comp.SpokenLanguages.Count)];
            var spoken = comp.SpokenLanguages.ToList();
            _random.Shuffle(spoken);
            comp.UnderstoodLanguages = [.. spoken.Take(comp.UnderstoodLanguages.Count())];
        }
        else
        {
            _random.Shuffle(allLangs);
            comp.UnderstoodLanguages = [.. allLangs.Take(comp.UnderstoodLanguages.Count)];
            var understood = comp.UnderstoodLanguages.ToList();
            _random.Shuffle(understood);
            comp.SpokenLanguages = [.. understood.Take(comp.SpokenLanguages.Count())];
        }

        if (
            comp.SpokenLanguages.Contains(SharedLanguageSystem.UniversalPrototype) ||
            comp.UnderstoodLanguages.Contains(SharedLanguageSystem.UniversalPrototype)
        )

            EnsureComp<UniversalLanguageSpeakerComponent>(languageKnower);
        if (TryComp<LanguageSpeakerComponent>(languageKnower, out var speaker))
            _language.UpdateEntityLanguages((languageKnower, speaker));
    }

    private void OnMapInit(Entity<TowerOfBabelComponent> ent, ref MapInitEvent ev)
    {
        var langs = _language.Languages.ToList();
        foreach (var languageKnower in EntityManager.AllEntities<LanguageKnowledgeComponent>())
        {
            ShuffleLanguages(languageKnower, langs);
        }
    }

    private void OnDestruction(Entity<TowerOfBabelComponent> ent, ref DestructionEventArgs ev)
    {
        foreach (var languageKnower in EntityManager.AllEntities<LanguageKnowledgeComponent>())
        {
            _language.RestoreCache((languageKnower, EnsureComp<LanguageCacheComponent>(languageKnower)));
            if (TryComp<LanguageSpeakerComponent>(languageKnower, out var speaker))
                _language.UpdateEntityLanguages((languageKnower, speaker));
        }
    }

    private void OnLanguageKnowledgeInit(ref LanguageKnowledgeInitEvent ev)
    {
        var ent = ev.Entity;
        ShuffleLanguages(ent);
    }
}