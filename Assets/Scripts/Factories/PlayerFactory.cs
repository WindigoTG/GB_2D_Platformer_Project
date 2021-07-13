using UnityEngine;

public class PlayerFactory : IPlayerFactory
{
    MainController _mainController;

    public PlayerFactory(MainController mainController)
    {
        _mainController = mainController;
    }

    public PlayerModel CreatePlayer()
    {
        return new PlayerModel(CreateUntransformedForm(), CreateTransformedForm());
    }

    private PlayerView CreateUntransformedForm()
    {
        SpriteAnimatorController animatorController = new SpriteAnimatorController(
                                                        Resources.Load<SpriteAnimationsConfig>("VentAnimationsConfig"));
        _mainController.AddUpdatable(animatorController);
        Transform transform = Object.Instantiate(Resources.Load<GameObject>("PlayerUntransformed")).transform;
        return new UntransformedPlayerView(animatorController, transform);
    }

    private PlayerView CreateTransformedForm()
    {
        SpriteAnimatorController animatorController = new SpriteAnimatorController(
                                                        Resources.Load<SpriteAnimationsConfig>("ModelXAnimationsConfig"));
        _mainController.AddUpdatable(animatorController);
        Transform transform = Object.Instantiate(Resources.Load<GameObject>("PlayerTransformed")).transform;
        return new TransformedPlayerView(animatorController, transform);
    }
}
