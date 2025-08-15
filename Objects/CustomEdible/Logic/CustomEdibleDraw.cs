using RWCustom;

namespace CustomEdible;

public partial class CustomEdible
{

    public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[] { new FSprite("Circle20") { scale = 1f } };
        AddToContainer(sLeaser, rCam, null);
    }

    public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        Vector2 pos = Vector2.Lerp(firstChunk.lastPos, firstChunk.pos, timeStacker);
        Vector2 rt = Vector3.Slerp(lastRotation, rotation, timeStacker);
        lastDarkness = darkness;
        //The formula for determining darkness is a template
        darkness = rCam.room.Darkness(pos) * (1f - rCam.room.LightSourceExposure(pos));
        if (darkness != lastDarkness)
            ApplyPalette(sLeaser, rCam, rCam.currentPalette);
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

    public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        //You don't have to assign a value to this variable, you can specify the colors for each sprite separately
        color = Color.blue;
        color = Color.Lerp(color, palette.blackColor, darkness); //The darker it is (the closer the darkness value is to 1f), the darker the sprite will be
        sLeaser.sprites[0].color = color;
    }

    public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
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

