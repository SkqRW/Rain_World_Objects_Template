
namespace CustomEdible;

internal class Hooks
{
    public static void Terminate()
    {
        // Unregister any hooks or cleanup resources here if necessary
        On.Player.Grabability -= Player_Grabability;
    }
    public static void Init()
    {
        //Here you can put your custom hooks for your objects
        On.Player.Grabability += Player_Grabability;
    }

    private static Player.ObjectGrabability Player_Grabability(On.Player.orig_Grabability orig, Player self, PhysicalObject obj)
    {
        // Here you define the conditions under which your custom object will be grabbed by the slugcat
        // The next are the option for selecting the grabability of the object
        /*
        CantGrab,
        OneHand, // Allow the slugcat to grab the object with one hand
		BigOneHand, //The same as OneHand, but the slugcat can't have two of these objects at the same time
		TwoHands, //The object have to be grabbed with both hands
		Drag //The object can be dragged, grabbed with both hands, (can this be throw?)
        */
        //You can define custom grabilityes depedending custom conditions, are free for creativity :D

        if (obj is CustomEdible)
            return Player.ObjectGrabability.OneHand;
        return orig(self, obj);
    }
}

