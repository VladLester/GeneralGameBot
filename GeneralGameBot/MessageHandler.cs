using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralGameBot
{
    static class MessageHandler
    {
        public static readonly string DefaultGeneralPhotoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/9/98/Stalin_in_March_1935.jpg/1200px-Stalin_in_March_1935.jpg";

        public static void ChangeName(Entities.General general,string message)
        {
            try
            {
                using (AppContext context = new AppContext())
                {
                    string NewName = message.Replace("Имя:", "");
                    context.Update(general);
                    general.Name = NewName;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }           
        }
        public static void ChangePhoto(Entities.General general, string message)
        {
            try
            {
                using (AppContext context = new AppContext())
                {
                    string NewPhotoUrl = message.Replace("Фото:", "");
                    context.Update(general);
                    general.PhotoUrl = NewPhotoUrl;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message); 
            }     
        }
        
        public static Entities.General AnotherGeneralInfo(string message)
        {
            string anotherUsername = message.Replace("Генерал другого человека:", "");
            return GameDataBase.GetGeneral(anotherUsername);
        }
    }


}
