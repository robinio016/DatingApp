using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;


namespace DatingApp.API.Data
{
    public interface IDatingRepository
    {
         void Add<T>(T entity) where T: class;
         void Delete<T> (T entity) where T: class;
         Task<bool> SaveAll(); // retrun true when all values has been saved to database
         // Task<IEnumerable<User>> GetUsers();
         Task<PagedList<User>> GetUsers(UserParams userParams); 
         Task<User> GetUser(int Id);
         Task<Photo> GetPhoto(int id);
         Task<Photo> GetMainPhotoForUser(int userId);

         Task<Like> GetLike(int userId, int recipientId);

         Task<Message> GetMessage (int id);
         Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams);
         Task<IEnumerable<Message>> GetMessageThread(int userId,int recipientId);
    }
}