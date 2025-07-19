
using RWCustom;
using UnityEngine;

namespace SpikePlant;

public partial class SpikePlant
{

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[2];
        sLeaser.sprites[0] = new FSprite("SmallSpear");

        TriangleMesh.Triangle[] tris = new TriangleMesh.Triangle[]
            { new TriangleMesh.Triangle(0, 1, 2)};
        TriangleMesh triangleMesh = new TriangleMesh("Futile_White", tris, false, false);
        sLeaser.sprites[1] = triangleMesh;

        AddToContainer(sLeaser, rCam, null);
    }

    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    { 


        Vector2 pos = Vector2.Lerp(firstChunk.lastPos, firstChunk.pos, timeStacker);
        Vector2 rt = Vector3.Slerp(lastRotation, rotation, timeStacker);
        lastDarkness = darkness;
        //The formula for determining darkness is a template
        darkness = rCam.room.Darkness(pos) * (1f - rCam.room.LightSourceExposure(pos));
        if (darkness != lastDarkness)
            ApplyPalette(sLeaser, rCam, rCam.currentPalette);


        sLeaser.sprites[0].x = pos.x - camPos.x;
        sLeaser.sprites[0].y = pos.y - camPos.y;
        sLeaser.sprites[0].rotation = Custom.VecToDeg(rt);

        (sLeaser.sprites[1] as TriangleMesh).MoveVertice(0, pos + new Vector2(0f, 0f) - camPos);
        (sLeaser.sprites[1] as TriangleMesh).MoveVertice(1, pos + new Vector2(10f, 10f) - camPos);
        (sLeaser.sprites[1] as TriangleMesh).MoveVertice(2, pos + new Vector2(20f, 0f) - camPos);


        //If your object is PlayerCarryableItem, then when approaching an object, it can "flash" in one frame, hinting that it can be grabbed
        if (blink > 0 && UnityEngine.Random.value < 0.5f)
            sLeaser.sprites[0].color = blinkColor;
        else sLeaser.sprites[0].color = color;

       

        if (slatedForDeletetion || rCam.room != room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }

    public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        //You don't have to assign a value to this variable, you can specify the colors for each sprite separately
        color = Color.cyan;
        color = Color.Lerp(color, palette.blackColor, darkness); //The darker it is (the closer the darkness value is to 1f), the darker the sprite will be
        sLeaser.sprites[0].color = color;
    }

    public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        
        //If newContainer is null, then it is assigned an Items container
        newContatiner = newContatiner ?? rCam.ReturnFContainer("Items");
        foreach (FSprite sprite in sLeaser.sprites)
        {
            sprite.RemoveFromContainer();
            newContatiner.AddChild(sprite);
        }
    }
}

