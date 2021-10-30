using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralGameBot
{
    class GameDataBase
    {
        public static bool DataBaseContains(string username)
        {

            using (AppContext context = new AppContext())
            {
                foreach (var item in context.Generals)
                {
                    if (item.TUsername == username)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void DataBaseAdd(Entities.General general)
        {
            try
            {
                using (AppContext context = new AppContext())
                {
                    context.Add(general);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            
        }

        public static Entities.General GeneralCreate(string username)
        {
            
            Entities.General general = new Entities.General() { TUsername = username };
            return general;


        }

        public static Entities.General GetGeneral(string username)
        {
            using (AppContext context = new AppContext())
            {
                foreach (var item in context.Generals)
                {
                    if (item.TUsername == username)
                    {
                        return item;
                    }
                }
            }

            return null;
            
        }
    }
}
