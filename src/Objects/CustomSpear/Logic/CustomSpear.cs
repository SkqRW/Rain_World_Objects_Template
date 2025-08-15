using System;
using MoreSlugcats;
namespace CustomSpear;

//Strongly inspired in the dangleFruit logic
public partial class CustomSpear : Spear
{
    //These are necessary variables that will be useful to us later
    public Vector2 rotation;
    public Vector2 lastRotation;
    public float darkness;
    public float lastDarkness;

    public CustomSpear(AbstractPhysicalObject abstr, World world) : base(abstr, world)
    {
        abstractPhysicalObject = abstr as AbstractSpear;
        base.bodyChunks = new BodyChunk[1];
        base.bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0f, 0f), 5.5f, 0.2f);
        bodyChunkConnections = new PhysicalObject.BodyChunkConnection[0];
        gravity = 0.9f;
        airFriction = 0.999f;
        waterFriction = 0.98f;
        surfaceFriction = 0.4f;
        collisionLayer = 1;
        bounce = 0.4f;
        buoyancy = 0.9f;
        firstChunk.loudness = 4f;
        ODEBUG.LogInfo($"Creating object {this}");
    }

    public override void Update(bool eu)
    {
        base.Update(eu);
        lastRotation = rotation;
        if (grabbedBy.Count > 0)
        {
            rotation = Custom.PerpendicularVector(Custom.DirVec(firstChunk.pos, grabbedBy[0].grabber.mainBodyChunk.pos));
            rotation.y = Mathf.Abs(rotation.y);
        }
        if (firstChunk.contactPoint.y < 0) //If the object touches the ground
        {
            rotation = (rotation - Custom.PerpendicularVector(rotation) * 0.1f * firstChunk.vel.x).normalized;
            firstChunk.vel.x *= 0.8f;
        }
    }

    public override void PlaceInRoom(Room placeRoom)
    {
        try
        {
            ODEBUG.Log("0  placetoom");
            placeRoom.AddObject(this);
            ODEBUG.Log("1  placetoom");
            if (placeRoom.terrain != null)
            {
                this.Buried = true;
            }
            ODEBUG.Log("2  placetoom");
            firstChunk.HardSetPosition(placeRoom.MiddleOfTile(abstractPhysicalObject.pos.Tile));

            ODEBUG.Log("3  placetoom");
            rotation = Custom.RNV();
            ODEBUG.Log("4  placetoom");
            lastRotation = rotation;
            ODEBUG.Log("5  placetoom");
        }
        catch (Exception e)
        {
            ODEBUG.LogWarn("sPEARScUSTOM Here D: " + e.Message);
        }
    }

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[1];
        sLeaser.sprites[0] = new FSprite("Circle20") { scale = 1f };
        UnityEngine.Debug.Log($"Initiating sprites for {this}");
        AddToContainer(sLeaser, rCam, null);
    }

    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        Vector2 pos = Vector2.Lerp(firstChunk.lastPos, firstChunk.pos, timeStacker);
        Vector2 rt = Vector3.Slerp(lastRotation, rotation, timeStacker);
        lastDarkness = darkness;
        //The formula for determining darkness is a template
        darkness = rCam.room.Darkness(pos) * (1f - rCam.room.LightSourceExposure(pos));
        foreach (FSprite sprite in sLeaser.sprites)
        {
            sprite.x = pos.x - camPos.x;
            sprite.y = pos.y - camPos.y;
            sprite.rotation = Custom.VecToDeg(rt);
        }
        //If your object is PlayerCarryableItem, then when approaching an object, it can "flash" in one frame, hinting that it can be grabbed
        if (blink > 0 && UnityEngine.Random.value < 0.5f)
            sLeaser.sprites[0].color = blinkColor;
        else sLeaser.sprites[0].color = color;
        if (slatedForDeletetion || rCam.room != room)
            sLeaser.CleanSpritesAndRemove();
    }
}

