using System;
using MoreSlugcats;
namespace SpikePlant;

//Strongly inspired in the dangleFruit logic
public partial class SpikePlant : Weapon, IPlayerEdible
{
    //These are necessary variables that will be useful to us later
    public Vector2 rotation;
    public Vector2 lastRotation;
    public float darkness;
    public float lastDarkness;

    public SpikePlant(AbstractPhysicalObject abstr, World world) : base(abstr, world)
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
        //ODEBUG.Log("SHOW THIS IS UPDATE O");
    }
     
    public override void PlaceInRoom(Room placeRoom)
    {
        try
        {
            base.PlaceInRoom(placeRoom);
            if (ModManager.MMF && room.game.IsArenaSession && (MMF.cfgSandboxItemStems.Value || room.game.GetArenaGameSession.chMeta != null) && room.game.GetArenaGameSession.counter < 10)
            {
                base.firstChunk.HardSetPosition(placeRoom.MiddleOfTile(abstractPhysicalObject.pos));
                stalk = new SpikePlant.Stalk(this, placeRoom, base.firstChunk.pos);
                placeRoom.AddObject(stalk);
            }
            else if (!AbstrConsumable.isConsumed && AbstrConsumable.placedObjectIndex >= 0 && AbstrConsumable.placedObjectIndex < placeRoom.roomSettings.placedObjects.Count)
            {
                base.firstChunk.HardSetPosition(placeRoom.roomSettings.placedObjects[AbstrConsumable.placedObjectIndex].pos);
                stalk = new SpikePlant.Stalk(this, placeRoom, base.firstChunk.pos);
                placeRoom.AddObject(stalk);
            }
            else
            {
                base.firstChunk.HardSetPosition(placeRoom.MiddleOfTile(abstractPhysicalObject.pos));
                rotation = Custom.RNV();
                lastRotation = rotation;
            }
        }
        catch (Exception e)
        {
            ODEBUG.LogErr(e.Message);
        }
        
        /*
        //The basic method is actually just adding an entity to the room, and you can do it manually if you want
        base.PlaceInRoom(placeRoom);
        //Places an object according to the coordinates of its abstractPhysicalObject
        firstChunk.HardSetPosition(placeRoom.MiddleOfTile(abstractPhysicalObject.pos.Tile));
        //Custom.RNV() just sets a random direction
        rotation = Custom.RNV();
        lastRotation = rotation;

        stalk = new SpikePlant.Stalk(this, placeRoom, base.firstChunk.pos);
        placeRoom.AddObject(stalk);

        
        { 
            //The basic method is actually just adding an entity to the room, and you can do it manually if you want
            base.PlaceInRoom(placeRoom);
            //Places an object according to the coordinates of its abstractPhysicalObject
            firstChunk.HardSetPosition(placeRoom.MiddleOfTile(abstractPhysicalObject.pos.Tile));
            //Custom.RNV() just sets a random direction
            rotation = Custom.RNV();
            lastRotation = rotation;
         * 
         */
    }

    public void DetatchStalk()
    {
        if (this.stalk != null && this.stalk.releaseCounter == 0)
        {
            this.stalk.releaseCounter = 2;
        }
    }
    public Stalk stalk;

    public AbstractConsumable AbstrConsumable => abstractPhysicalObject as AbstractConsumable;
    
}

