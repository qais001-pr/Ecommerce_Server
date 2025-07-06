using MongoDB.Driver;

namespace ECommerce_App.Services
{
    public interface IAdminService
    {
        Task CreateAsync(Admin admin);
        Task <bool> Login(string email, string password);
        Task<AdminFormRequest> GetAdmin(string email);
    }

    public class AdminService : IAdminService
    {
        private readonly IMongoCollection<Admin> _adminCollection;

        public AdminService(IMongoDatabase database)
        {
            _adminCollection = database.GetCollection<Admin>("Admins");
        }
        public async Task<bool> Login(string email, string pasword)
        {
            var checkLogin = await _adminCollection.Find(u => u.Email == email && u.Password == pasword).FirstOrDefaultAsync();
            if (checkLogin == null)
            {
                return false;
            }
            return true;
        }
        public async Task<AdminFormRequest?> GetAdmin(string email)
        {
            var admin = await _adminCollection.Find(e => e.Email == email).FirstOrDefaultAsync();

            if (admin == null)
                return null;

            var adminFormRequest = new AdminFormRequest
            {
                id = admin.Id,
                Name = admin.Name,
                Email = admin.Email,
                Username = admin.Username,
                Gender = admin.Gender,
                ImageBytes = admin.ImageBytes,
                ContactNo = admin.ContactNo,
                Password = admin.Password,
                Content = admin.Content,
            };

            return adminFormRequest;
        }


        public async Task CreateAsync(Admin admin)
        {
            if (admin == null) throw new ArgumentNullException(nameof(admin));

            // Validate required fields
            if (string.IsNullOrWhiteSpace(admin.Username))
                throw new ArgumentException("Username is required");

            if (string.IsNullOrWhiteSpace(admin.Email))
                throw new ArgumentException("Email is required");

            // Check for existing admin
            var existingAdmin = await _adminCollection.Find(a =>
                a.Username == admin.Username || a.Email == admin.Email)
                .FirstOrDefaultAsync();

            if (existingAdmin != null)
            {
                throw new InvalidOperationException(
                    existingAdmin.Username == admin.Username
                        ? "Username already exists"
                        : "Email already exists");
            }

            await _adminCollection.InsertOneAsync(admin);
        }
    }
}
