using UnityEngine;

public class AIController : IUpdateableFixed
{
    SimplePatrolAI[] _simpleAIs;
    PathingAI[] _pathingAIs;

    public AIController(PatrolData[] simplePatrolData, PatrolData[] stalkerPatrolData, SpriteAnimatorController enemyAnimatorController, SpriteAnimatorController explosionAnimatorController, Transform target)
    {
        _simpleAIs = new SimplePatrolAI[simplePatrolData.Length];
        _pathingAIs = new PathingAI[stalkerPatrolData.Length];
        var enemyPrefab = Resources.Load<GameObject>("TellyBomb");
        var explosionPrefab = Resources.Load<GameObject>("Explosion");

        AIConfig config = Resources.Load<ScriptableAIConfig>("AIConfig").Config;

        for (int i = 0; i < _simpleAIs.Length; i++)
        {
            var viewObject = Object.Instantiate(enemyPrefab);
            enemyAnimatorController.StartAnimation(viewObject.GetComponent<SpriteRenderer>(), Track.Idle, true, 10);
            SimplePatrolAIView view = new SimplePatrolAIView(viewObject);
            config.PatrolData = simplePatrolData[i];
            SimplePatrolAIModel model = new SimplePatrolAIModel(config);

            _simpleAIs[i] = new SimplePatrolAI(view, model, target, explosionPrefab, explosionAnimatorController);
        }

        enemyPrefab = Resources.Load<GameObject>("PathingTellyBomb");
        for (int i = 0; i < _pathingAIs.Length; i++)
        {
            var viewObject = Object.Instantiate(enemyPrefab);
            enemyAnimatorController.StartAnimation(viewObject.GetComponent<SpriteRenderer>(), Track.Idle, true, 10);
            PathingAIView view = new PathingAIView(viewObject);
            config.PatrolData = stalkerPatrolData[i];
            PathingAIModel model = new PathingAIModel(config);

            _pathingAIs[i] = new PathingAI(view, model, target, explosionPrefab, explosionAnimatorController);
        }
    }

    public void FixedUpdate()
    {
        for (int i = 0; i < _simpleAIs.Length; i++)
            _simpleAIs[i].FixedUpdate();

        for (int i = 0; i < _pathingAIs.Length; i++)
            _pathingAIs[i].FixedUpdate();
    }

    public void RegisterTarget(PlayerHealth playerHealth)
    {
        for (int i = 0; i < _simpleAIs.Length; i++)
            _simpleAIs[i].TargetHit += playerHealth.TakeDamage;

        for (int i = 0; i < _pathingAIs.Length; i++)
            _pathingAIs[i].TargetHit += playerHealth.TakeDamage;
    }
}
