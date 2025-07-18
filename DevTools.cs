using UnityEngine;
using RWCustom;

namespace Plugin;

public partial class DevTools
{
    public static void Init()
    {
        On.Player.Update += PlayerOnUpdate;
        On.PlayerGraphics.DrawSprites += VisalFormToSeeIfTheModIsAplyed;
    }

    public static void Terminate()
    {
        On.Player.Update -= PlayerOnUpdate;
        On.PlayerGraphics.DrawSprites -= VisalFormToSeeIfTheModIsAplyed;
    }

    private static void VisalFormToSeeIfTheModIsAplyed(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        orig(self, sLeaser, rCam, timeStacker, camPos);
        sLeaser.sprites[4].color = Color.cyan;
    }

    public static void Log(string message)
    {
        UnityEngine.Debug.Log($"[Object] {message}");
    }

    public static void LogInfo(string message)
    {
        UnityEngine.Debug.Log($"[INFO Object] {message}");
    }

    public static void LogWarn(string message)
    {
        UnityEngine.Debug.Log($"[WARN Object] {message}");
    }

    public static void LogErr(string message)
    {
        UnityEngine.Debug.Log($"[ERROR Object] {message}");
    }

    private static bool DevMode = false;
    private static int devTimer = 0;

    public static void PlayerOnUpdate(On.Player.orig_Update orig, Player self, bool eu)
    {
        orig(self, eu);

        /*
        if (Input.GetKeyDown(KeyCode.D))
        {
            DevMode = true;
        }

        if (!DevMode)
        {
            return;
        }
        */
    
        devTimer++;


        if (Input.GetKey(KeyCode.Q) && devTimer > 0)
        {
            AbstractPhysicalObject temp = new AbstractPhysicalObject(
                self.room.world, 
                CustomEdible.Register.CustomEdible, 
                null, 
                self.room.GetWorldCoordinate(self.mainBodyChunk.pos), 
                self.room.game.GetNewID());

            CustomEdible.CustomEdible attemp = new CustomEdible.CustomEdible(temp);

            attemp.PlaceInRoom(self.room);
            ODEBUG.Log("the object placed in the room");
            SetDevTimer(1);
        }
    }

    //cuenta regresiva en secundos
    private static void SetDevTimer(int seconds)
    {
        devTimer = -seconds * 40;
    }
}
