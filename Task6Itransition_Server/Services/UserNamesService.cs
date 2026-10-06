using Domain.Entities;

namespace Task6Itransition_Server.Services
{
    public static class UserNamesService
    {
        private static Dictionary<string, Dictionary<Guid, UserName>> userNamesWithPostfixByIdByMap = new();

        public static Dictionary<Guid, UserName> Add(string mapName, string userName, Guid userId)
        {
            Dictionary<Guid, UserName> userNamesWithPostfixById;

            if(userNamesWithPostfixByIdByMap.ContainsKey(mapName)) userNamesWithPostfixById = userNamesWithPostfixByIdByMap[mapName];
            else
            {
                userNamesWithPostfixById = new();
                userNamesWithPostfixByIdByMap.Add(mapName, userNamesWithPostfixById);
            }

            var postfixs = userNamesWithPostfixById.Values
                .Where(x => x.Name == userName)
                .Select(x => x.Postfix)
                .OrderByDescending(x => x);

            var newPostfix = postfixs.Count() != 0 ? postfixs.First() + 1 : 0;

            userNamesWithPostfixById.Add(userId, new UserName { Name = userName, Postfix = newPostfix });
            return userNamesWithPostfixById;
        }

        public static Dictionary<Guid, UserName> Remove(string mapName, Guid userId)
        {
            Dictionary<Guid, UserName> userNamesWithPostfixById = userNamesWithPostfixByIdByMap[mapName];

            var removedUser = userNamesWithPostfixById[userId];

            var changedUsers = userNamesWithPostfixById.Values
                .Where(x => x.Name == removedUser.Name && x.Postfix > removedUser.Postfix);

            foreach (var user in changedUsers)
            {
                user.Postfix--;
            }

            userNamesWithPostfixById.Remove(userId);

            return userNamesWithPostfixById;
        }
    }
}
