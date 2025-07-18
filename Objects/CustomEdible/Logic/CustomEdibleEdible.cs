
namespace CustomEdible;

public partial class CustomEdible
{
    private int bites = 2;

    public int FoodPoints
    {
        get
        {
            return 1;
        }
    }

    public bool Edible
    {
        get
        {
            return true;
        }
    }

    public bool AutomaticPickUp
    {
        get
        {
            return false;
        }
    }

    public int BitesLeft
    {
        get
        {
            return this.bites;
        }
    }

    public void BitByPlayer(Creature.Grasp grasp, bool eu)
    {
        this.bites--;
        this.room.PlaySound((this.bites == 0) ? SoundID.Slugcat_Eat_Dangle_Fruit : SoundID.Slugcat_Bite_Dangle_Fruit, base.firstChunk);
        base.firstChunk.MoveFromOutsideMyUpdate(eu, grasp.grabber.mainBodyChunk.pos);
        if (this.bites <= 0)
        {
            (grasp.grabber as Player).ObjectEaten(this);
            grasp.Release();
            //this.Destroy();
        }
    }

    public void ThrowByPlayer()
    {
        //This function not work
        //But you need to put this empty for the edible interfaz work
    }
}

