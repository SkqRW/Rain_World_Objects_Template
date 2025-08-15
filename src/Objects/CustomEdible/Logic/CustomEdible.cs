using System;
using MoreSlugcats;
namespace CustomEdible;

//Strongly inspired in the dangleFruit logic
public partial class CustomEdible : PlayerCarryableItem, IPlayerEdible, IDrawable
{
    //These are necessary variables that will be useful to us later
    public Vector2 rotation;
    public Vector2 lastRotation;
    public float darkness;
    public float lastDarkness;

    public CustomEdible(AbstractPhysicalObject abstr) : base(abstr)
    {
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
            //The basic method is actually just adding an entity to the room, and you can do it manually if you want
            base.PlaceInRoom(placeRoom);
            //Places an object according to the coordinates of its abstractPhysicalObject
            firstChunk.HardSetPosition(placeRoom.MiddleOfTile(abstractPhysicalObject.pos.Tile));
            //Custom.RNV() just sets a random direction
            rotation = Custom.RNV();
            lastRotation = rotation;
        }
        catch(Exception e)
        {
            ODEBUG.LogWarn(e.Message);
        }
    }
    public AbstractConsumable AbstrConsumable => abstractPhysicalObject as AbstractConsumable;
    
}

