
namespace CustomEdible;

internal class Object
{
    private static string GUID;
    public static void Init(string guid)
    {
        GUID = guid;
        Register.RegisterValues();

        On.RainWorld.OnModsEnabled += RainWorld_OnModsEnabled;
        On.RainWorld.OnModsDisabled += RainWorld_OnModsDisabled;
        On.AbstractPhysicalObject.Realize += AbstractPhysicalObject_Realize;
        On.Player.Grabability += Player_Grabability;
        On.AbstractPhysicalObject.Realize += AbstractPhysicalObject_RealizeCustom;
        On.DevInterface.ObjectsPage.DevObjectGetCategoryFromPlacedType += ObjectsPage_DevObjectGetCategoryFromPlacedType;
        On.DevInterface.ObjectsPage.CreateObjRep += ObjectsPage_CreateObjRep;
        On.PlacedObject.ConsumableObjectData.ctor += ConsumableObjectData_ctor;
        On.PlacedObject.GenerateEmptyData += PlacedObject_GenerateEmptyData;
        On.Room.Loaded += Room_Loaded;
    }

    public static void Terminate()
    {
        Register.UnregisterValues();

        On.RainWorld.OnModsEnabled -= RainWorld_OnModsEnabled;
        On.RainWorld.OnModsDisabled -= RainWorld_OnModsDisabled;
        On.AbstractPhysicalObject.Realize -= AbstractPhysicalObject_Realize;
        On.Player.Grabability -= Player_Grabability;
        On.AbstractPhysicalObject.Realize -= AbstractPhysicalObject_RealizeCustom;
        On.Room.Loaded -= Room_Loaded;

        On.DevInterface.ObjectsPage.DevObjectGetCategoryFromPlacedType -= ObjectsPage_DevObjectGetCategoryFromPlacedType;
        On.DevInterface.ObjectsPage.CreateObjRep -= ObjectsPage_CreateObjRep;
        On.PlacedObject.ConsumableObjectData.ctor -= ConsumableObjectData_ctor;
        On.PlacedObject.GenerateEmptyData -= PlacedObject_GenerateEmptyData;
    }

    private static void RainWorld_OnModsEnabled(On.RainWorld.orig_OnModsEnabled orig, RainWorld self, ModManager.Mod[] newlyEnabledMods)
    {
        orig(self, newlyEnabledMods);
        Register.RegisterValues();
    }

    private static void RainWorld_OnModsDisabled(On.RainWorld.orig_OnModsDisabled orig, RainWorld self, ModManager.Mod[] newlyDisabledMods)
    {
        orig(self, newlyDisabledMods);
        foreach (ModManager.Mod mod in newlyDisabledMods)
            if (mod.id == GUID) 
                Register.UnregisterValues();
    }

    private static void AbstractPhysicalObject_Realize(On.AbstractPhysicalObject.orig_Realize orig, AbstractPhysicalObject self)
    {
        orig(self);
        if (self.type == Register.CustomEdible)
            self.realizedObject = new CustomEdible(self); //Like any physical object, your object will take an abstract object as a parameter. This will be written in more detail later
    }

    private static Player.ObjectGrabability Player_Grabability(On.Player.orig_Grabability orig, Player self, PhysicalObject obj)
    {
        //You can also specify the conditions under which there will be different options for grabbing the object. For example, depending on the weight of the item
        if (obj is CustomEdible)
            return Player.ObjectGrabability.OneHand;
        return orig(self, obj);
    }

    private static void Room_Loaded(On.Room.orig_Loaded orig, Room self)
    {
        orig(self);

        if (self.game == null)
            return;

        for (int num20 = 0; num20 < self.roomSettings.placedObjects.Count; num20++)
        {
            var po = self.roomSettings.placedObjects[num20];

            if (self.roomSettings.placedObjects[num20].type == Register.CustomEdible_PO)
            {

                if (!(self.game.session is StoryGameSession) ||
                    !(self.game.session as StoryGameSession).saveState.ItemConsumed(self.world, false, self.abstractRoom.index, num20))
                {
                    AbstractPhysicalObject abstractPhysicalObject = new AbstractConsumable(
                        self.world,
                        Register.CustomEdible,
                        null,
                        self.GetWorldCoordinate(self.roomSettings.placedObjects[num20].pos),
                        self.game.GetNewID(),
                        self.abstractRoom.index,
                        num20,
                        self.roomSettings.placedObjects[num20].data as PlacedObject.ConsumableObjectData);
                    (abstractPhysicalObject as AbstractConsumable).isConsumed = false;
                    self.abstractRoom.AddEntity(abstractPhysicalObject);
                    abstractPhysicalObject.placedObjectOrigin = self.SetAbstractRoomAndPlacedObjectNumber(self.abstractRoom.name, num20);

                }
            }
        }
    }

    private static void PlacedObject_GenerateEmptyData(On.PlacedObject.orig_GenerateEmptyData orig, PlacedObject self)
    {
        orig(self);
        if (self.type == Register.CustomEdible_PO) //The registered name of your PlacedObject.Type
            self.data = new PlacedObject.ConsumableObjectData(self);
    }

    private static void ConsumableObjectData_ctor(On.PlacedObject.ConsumableObjectData.orig_ctor orig, PlacedObject.ConsumableObjectData self, PlacedObject owner)
    {
        if (owner.type == Register.CustomEdible_PO)
        {
            self.minRegen = 1;
            self.maxRegen = 7;
        }
        else orig(self, owner);
    }

    private static void ObjectsPage_CreateObjRep(On.DevInterface.ObjectsPage.orig_CreateObjRep orig, DevInterface.ObjectsPage self, PlacedObject.Type tp, PlacedObject pObj)
    {
        if (tp == Register.CustomEdible_PO)
        {
            if (pObj == null)
            {
                pObj = new PlacedObject(tp, null)
                { pos = self.owner.room.game.cameras[0].pos + Vector2.Lerp(self.owner.mousePos, new Vector2(-683f, 384f), 0.25f) + RWCustom.Custom.DegToVec(UnityEngine.Random.value * 360f) * 0.2f };
                self.RoomSettings.placedObjects.Add(pObj);
            }
            DevInterface.PlacedObjectRepresentation por = new DevInterface.ConsumableRepresentation(self.owner, tp.ToString() + "_Rep", self, pObj, tp.ToString());
            self.tempNodes.Add(por);
            self.subNodes.Add(por);
        }
        else orig(self, tp, pObj);
    }

    private static DevInterface.ObjectsPage.DevObjectCategories ObjectsPage_DevObjectGetCategoryFromPlacedType(On.DevInterface.ObjectsPage.orig_DevObjectGetCategoryFromPlacedType orig, DevInterface.ObjectsPage self, PlacedObject.Type type)
    {
        if (type == Register.CustomEdible_PO)
            return DevInterface.ObjectsPage.DevObjectCategories.Consumable; //there are several categories, Consumable is one of them
        return orig(self, type);
    }

    private static void AbstractPhysicalObject_RealizeCustom(On.AbstractPhysicalObject.orig_Realize orig, AbstractPhysicalObject self)
    {
        orig(self);
        if (self.type == Register.CustomEdible)
            self.realizedObject = new CustomEdible(self); //Like any physical object, your object will take an abstract object as a parameter. This will be written in more detail later
    }
}

