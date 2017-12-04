using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace Iris.Api.Entities
{
    public class DataContext
    {
        public DataContext(string connectionString)
        {
            var url = new MongoUrl(connectionString);
            var client = new MongoClient(url);
            var database = client.GetDatabase(url.DatabaseName);

            GridFs = new GridFSBucket(database);
            Users = database.GetCollection<User>("users");
            Employees = database.GetCollection<Employee>("employees");
        }

        public GridFSBucket GridFs { get; set; }
        public IMongoCollection<User> Users { get; set; }
        public IMongoCollection<Employee> Employees { get; set; }

        public void EnsureIndexes()
        {
        }

        public void EnsureAdminAccount()
        {
            var admin = Users.Find(t => t.Username == "admin").FirstOrDefault();
            if (admin == null)
            {
                admin = new User();
                admin.Username = "admin";
                admin.Password = PasswordHasher.HashPassword("admin");
                admin.IsActive = true;
                Users.InsertOne(admin);
            }
        }

        public static void Init()
        {
            BsonClassMap.RegisterClassMap<BaseEntity>(cm =>
            {
                cm.AutoMap();
                cm.SetIdMember(cm.GetMemberMap(c => c.Id)
                    .SetSerializer(new StringSerializer(BsonType.ObjectId))
                    .SetIdGenerator(StringObjectIdGenerator.Instance));
            });

            BsonClassMap.RegisterClassMap<User>(cm =>
            {
                cm.AutoMap();
            });

            BsonClassMap.RegisterClassMap<UserClaim>(cm =>
            {
                cm.AutoMap();
            });
        }
    }
}