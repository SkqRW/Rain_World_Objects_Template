namespace CustomEdible;

public static class Register
{
    public static void RegisterValues()
    {
        CustomEdible = new AbstractPhysicalObject.AbstractObjectType("CustomEdible", true);
    }

    public static void UnregisterValues()
    {
        AbstractPhysicalObject.AbstractObjectType item = CustomEdible;
        item?.Unregister();
        item = null;
    }

    public static AbstractPhysicalObject.AbstractObjectType CustomEdible;

    public static readonly PlacedObject.Type CustomEdible_PO = new PlacedObject.Type("CustomEdible", register: true);
}
