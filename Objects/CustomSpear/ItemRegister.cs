namespace CustomSpear;

public static class Register
{
    public static void RegisterValues()
    {
        CustomSpear = new AbstractPhysicalObject.AbstractObjectType("CustomSpear", true);
    }

    public static void UnregisterValues()
    {
        AbstractPhysicalObject.AbstractObjectType item = CustomSpear;
        item?.Unregister();
        item = null;
    }

    public static AbstractPhysicalObject.AbstractObjectType CustomSpear;

    public static readonly PlacedObject.Type CustomSpear_PO = new PlacedObject.Type("CustomSpear", register: true);
}
