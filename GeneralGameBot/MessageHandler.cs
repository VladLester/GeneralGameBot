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
            using (AppContext context = new AppContext())
            {
                string NewName = message.Replace("Имя:", " ");
                context.Update(general);
                general.Name = NewName;
                context.SaveChanges();
            }
          
            
        }
        
    }


}
