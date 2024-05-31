namespace GUIClient
{
    public sealed class ManagerPlayers
    {
        public static int counter = 0;
        private List<int> usersId { get; set; } = new List<int>();

        public void addUserId()
        {
            counter++;
            usersId.Add(counter);
        }

        public int getUserId()
        {
            return counter;
        }

        public void deleteUserById(int userId)
        {
            usersId.Remove(userId);
        }

        public void deleteUserAtPosition(int index)
        {
            if (index >= 0 && index < usersId.Count)
            {
                usersId.RemoveAt(index);
            }
        }
        public void printUserIds()
        {
            foreach (var id in usersId)
            {
                Console.WriteLine(id);
            }
        }
    }
}
