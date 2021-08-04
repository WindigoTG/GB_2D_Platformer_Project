using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class QuestsConfigurator : IUpdateableFixed
{
    [SerializeField] private QuestChainConfig[] _questChainConfigs;
    [SerializeField] private QuestObjectPosition[] _questObjectPosition;

    [SerializeField] GameObject _questObjectPrefab;
    [SerializeField] Color _defaultQuestObjectColor;
    [SerializeField] Color _completedQuestObjectColor;

    Transform _targetPlayer;
    

    private readonly Dictionary<QuestType, Func<IQuestModel>> _questFactories = new Dictionary<QuestType, Func<IQuestModel>>
   {
       { QuestType.Switch, () => new SwitchQuestModel() },
   };

    private readonly Dictionary<QuestStoryType, Func<List<IQuest>, int, IQuestChain>> _questChainFactories = new Dictionary<QuestStoryType, Func<List<IQuest>, int, IQuestChain>>
   {
       { QuestStoryType.Common, (questCollection, id) => new QuestChain(questCollection, id) },
       { QuestStoryType.Resettable, (questCollection, id) => new ResettableQuestChain(questCollection, id) },
   };

    private List<IQuestChain> _questChains;
    private Quest _singleQuest;

    public void Initialize(Transform targetPlayer)
    {
        _targetPlayer = targetPlayer;

        _questChains = new List<IQuestChain>();
        foreach (var questChainConfig in _questChainConfigs)
        {
            _questChains.Add(CreateQuestChain(questChainConfig));
        }
    }

    private void OnDestroy()
    {
        foreach (var questStory in _questChains)
        {
            questStory.Dispose();
        }
        _questChains.Clear();
    }

    private IQuestChain CreateQuestChain(QuestChainConfig config)
    {
        var quests = new List<IQuest>();
        foreach (var questConfig in config.Quests)
        {
            var quest = CreateQuest(questConfig);
            if (quest == null) continue;
            quests.Add(quest);
        }
        return _questChainFactories[config.QuestChainType].Invoke(quests, config.Id);
    }

    private IQuest CreateQuest(QuestConfig config)
    {
        var questId = config.Id;
        var objectPosition = _questObjectPosition.FirstOrDefault(value => value.Id == config.Id);
        if (objectPosition == null)
        {
            Debug.LogWarning($"QuestsConfigurator: Can't find position of quest object {questId.ToString()}");
            return null;
        }

        var questObject = UnityEngine.Object.Instantiate(_questObjectPrefab, objectPosition.Position, Quaternion.identity);
        var questView = new QuestObjectView(questObject, _defaultQuestObjectColor, _completedQuestObjectColor);

        if (_questFactories.TryGetValue(config.QuestType, out var factory))
        {
            var questModel = factory.Invoke();
            return new Quest(questView, questModel, _targetPlayer);
        }

        Debug.LogWarning($"QuestsConfigurator :: Start : Can't create model for quest {questId.ToString()}");
        return null;
    }

    public void FixedUpdate()
    {
        for (int i = 0; i < _questChains.Count; i++)
            _questChains[i].Update();
    }

    public IQuestChain GetQuestChain(int id)
    {
        return _questChains.FirstOrDefault(value => value.Id == id);
    }
}

