public enum GameState
{
    Pause,
    Playing
}

public enum EnvironmentType
{
    none,
    dev,
    test,
    live
}

public enum ActionKeyType
{
    Stay,
    Up,
    Down
}

public enum ResponseInteractionType
{
    None,
    Quest,
    Quiz,
    Talk,
    Seat
}

public enum CurrencyType
{
    Ps,
    Prl
}

public enum SlotmachineItemType
{
    watermelon,
    cherry,
    bar,
    jackpot
}

public enum InventoryItemType
{
    currency,
    character,
    weapon,
    consumable,
    nonconsumable,
}

public enum CurrentRoomType
{
    None,
    Openworld,
    Class
}

public enum ClassRoomMediaType
{
    video,
    slide
}

public enum ClassRoomRemoteInteactionType
{
    Play = 1,
    Pause,
    Next,
    Back,
    Reset
}

public enum ClassRoomRole
{
    none,
    teacher,
    student
}

public enum NetworkAnimationValue
{
    IDLE = 0,
    WALK,
    RUN,
    JUMP,
    SIT,
    DRIVE,
    EMOTION1,
    EMOTION2,
    EMOTION3,
    EMOTION4,
    EMOTION5,
    EMOTION6,
    EMOTION7,
    EMOTION8,
}
public enum AnimHipHop
{
    HappyIdle = 0,
    Hiphopdance = 1,
    Victory = 2,
    Clap = 3,
    Standing = 4
}

public enum IAPItemType
{
    consumable,
    nonconsumable,
    supcription
}

public enum SceneType
{
    Splash,
    Loading,
    MainMenu,
    Gameplay
}

public enum LayerType
{
    Cloud = 7,
    PostProcessing = 9,
    Level_worldmap = 10
}