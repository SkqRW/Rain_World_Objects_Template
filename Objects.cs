using System;

namespace Plugin;

public partial class Objects
{
    public static void Init(string GUID)
    {
        // We used the mod id like a var to register and unregister the objects
        CustomEdible.Object.Init(GUID);
        On.Room.Loaded += Room_Loaded;
    }

    public static void Terminate()
    {
        CustomEdible.Object.Terminate();
        On.Room.Loaded -= Room_Loaded;
    }

    private static void Room_Loaded(On.Room.orig_Loaded orig, Room self)
    {
        //This is only add the objects in the first time the room is realized
        //Since the orig hook turn off this var, we read this before to ejecute the orig method
        bool firstTimeRealized = self.abstractRoom.firstTimeRealized;
        orig(self);

        if (self.game == null)
        {
            return;
        }

        if (firstTimeRealized)
        {
            for (int i = 0; i < self.roomSettings.placedObjects.Count; i++)
            {
                //Make sure that all your custom objects are here
                if (self.roomSettings.placedObjects[i].type == CustomEdible.Register.CustomEdible_PO)
                {
                    if (!(self.game.session is StoryGameSession) ||
                        !(self.game.session as StoryGameSession).saveState.ItemConsumed(self.world, false, self.abstractRoom.index, i))
                    {
                        AbstractPhysicalObject abstractPhysicalObject = new AbstractConsumable(
                            self.world,
                            CustomEdible.Register.CustomEdible,
                            null,
                            self.GetWorldCoordinate(self.roomSettings.placedObjects[i].pos),
                            self.game.GetNewID(),
                            self.abstractRoom.index,
                            i,
                            self.roomSettings.placedObjects[i].data as PlacedObject.ConsumableObjectData);
                        (abstractPhysicalObject as AbstractConsumable).isConsumed = false;
                        self.abstractRoom.AddEntity(abstractPhysicalObject);
                        abstractPhysicalObject.placedObjectOrigin = self.SetAbstractRoomAndPlacedObjectNumber(self.abstractRoom.name, i);

                    }
                }
            }
        }
    }
}
