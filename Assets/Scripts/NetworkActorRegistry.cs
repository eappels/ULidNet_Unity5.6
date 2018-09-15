public static class NetworkActorRegistry
{

    public static NetworkActor[] Objects = new NetworkActor[256];

    public static NetworkActor GetById(int id)
    {
        if (id < Objects.Length)
        {
            return Objects[id];
        }
        return null;
    }
}