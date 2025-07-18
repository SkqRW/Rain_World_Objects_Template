namespace SpikePlant;

public static class Register
{
    public static void RegisterValues()
    {
        SpikePlant = new AbstractPhysicalObject.AbstractObjectType("SpikePlant", true);
    }

    public static void UnregisterValues()
    {
        AbstractPhysicalObject.AbstractObjectType item = SpikePlant;
        item?.Unregister();
        item = null;
    }

    public static AbstractPhysicalObject.AbstractObjectType SpikePlant;

    public static readonly PlacedObject.Type SpikePlant_PO = new PlacedObject.Type("SpikePlant", register: true);
}
