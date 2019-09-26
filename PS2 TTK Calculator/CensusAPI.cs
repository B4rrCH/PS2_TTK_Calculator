using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace PS2_TTK_calculator
{
    public static class CensusAPI
    {
        private static string weaponDataURL = "http://census.daybreakgames.com/get/ps2/item?item_category_id=3,4,5,6,7,8,11,12,14,19,24&c:limit=500&c:lang=en&c:join=fire_mode,weapon_datasheet,item_to_weapon^inject_at:fire_mode_2(weapon,weapon_ammo_slot^on:weapon_id^to:weapon_id^list:1,weapon_to_fire_group^on:weapon_id^to:weapon_id^list:1(fire_group^on:fire_group_id^to:fire_group_id,fire_group_to_fire_mode^on:fire_group_id^to:fire_group_id^list:1(fire_mode_2^on:fire_mode_id^to:fire_mode_id^list:1(effect^on:damage_indirect_effect_id^to:effect_id^list:1(effect_type^list:1))))))";
        private static string weaponFilePath = "Data/weapons.json";

        private static JObject weaponDataJSON;
        private static List<Weapon> weaponList;
        private static string[] NYIweapons = { "6004198" };

        static CensusAPI()
        {
            LoadWeaponData();
            LoadWeaponList();
        }

        public static List<Weapon> GetWeaponList()
        {
            return weaponList;
        }

        public static void UpdateWeaponDataFromDBG()
        {
            using (var webClient = new WebClient())
            {
                if (File.Exists(weaponFilePath))
                {
                    webClient.DownloadFile(weaponDataURL, weaponFilePath);
                }
                else
                {
                    System.IO.Directory.CreateDirectory("Data");
                    File.CreateText(weaponFilePath).Close();
                    webClient.DownloadFile(weaponDataURL, weaponFilePath);
                }

            }
        }
        private static void LoadWeaponData()
        {
            if (!File.Exists(weaponFilePath))
            {
                UpdateWeaponDataFromDBG();
            }
            weaponDataJSON = JObject.Parse(File.ReadAllText(weaponFilePath));
        }
        private static void LoadWeaponList()
        {
            if (weaponDataJSON == null)
            {
                LoadWeaponData();
            }
            weaponList = new List<Weapon>();

            foreach (JToken weaponJToken in weaponDataJSON["item_list"])
            {
                if (!NYIweapons.Contains(weaponJToken.Value<string>("item_id")))
                {
                    weaponList.Add(new Weapon(weaponJToken));
                }
;
            }
        }

        private static int GetIndex(int item_id)
        {
            try
            {
                int index = 0;
                for (; index < weaponDataJSON.Value<int?>("returned"); ++index)
                {
                    if ((int)weaponDataJSON["item_list"][index]["item_id"] == item_id)
                    {
                        return index;
                    }
                }
            }
            catch (AccessViolationException) { return -1; };
            return -1;
        }

        private static JToken WeaponJTokenFromIndex(int index)
        {
            return weaponDataJSON["item_list"][index];
        }
        public static Weapon GetWeapon(int item_id)
        {
            if (weaponDataJSON == null)
            {
                LoadWeaponData();
            }
            int index = GetIndex(item_id);
            return new Weapon(WeaponJTokenFromIndex(index));
        }
    }
}
