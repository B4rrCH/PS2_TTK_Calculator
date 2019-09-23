using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace PS2_TTK_calculator
{
    public class CensusAPI
    {
        private static string weaponDataURL = "http://census.daybreakgames.com/get/ps2/item?item_category_id=3,5,6,7,8,11,12,14&c:limit=500&c:lang=en&c:join=fire_mode,weapon_datasheet,item_to_weapon^inject_at:fire_mode_2(weapon,weapon_ammo_slot^on:weapon_id^to:weapon_id^list:1,weapon_to_fire_group^on:weapon_id^to:weapon_id^list:1(fire_group^on:fire_group_id^to:fire_group_id,fire_group_to_fire_mode^on:fire_group_id^to:fire_group_id^list:1(fire_mode_2^on:fire_mode_id^to:fire_mode_id^list:1(effect^on:damage_indirect_effect_id^to:effect_id^list:1(effect_type^list:1))))))";
        private static string weaponFilePath = "Data/weapons.json";

        private JObject weaponDataJSON;
        private List<Weapon> weaponList;
        private string[] NYIweapons = { "6004198" };

        public CensusAPI()
        {
            LoadWeaponData();
            LoadWeaponList();
        }

        public List<Weapon> GetWeaponList()
        {
            return weaponList;
        }

        public void UpdateWeaponDataFromDBG()
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
        private void LoadWeaponData()
        {
            if (!File.Exists(weaponFilePath))
            {
                UpdateWeaponDataFromDBG();
            }
            weaponDataJSON = JObject.Parse(File.ReadAllText(weaponFilePath));
        }
        private void LoadWeaponList()
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
        public int NumberOfWeapons()
        {
            return weaponList.Count();
        }

        private int GetIndex(int item_id)
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
        private int GetIndex(string search)
        {
            try
            {
                int index = 0;
                for (; index < 500; ++index)
                {
                    if (((string)weaponDataJSON["item_list"][index]["name"]["en"]).Contains(search))
                    {
                        return index;
                    }
                }
            }
            catch (AccessViolationException) { return -1; };
            return -1;
        }

        private JToken WeaponJTokenFromIndex(int index)
        {
            return weaponDataJSON["item_list"][index];
        }
        public Weapon GetWeapon(int item_id)
        {
            if (weaponDataJSON == null)
            {
                LoadWeaponData();
            }
            int index = GetIndex(item_id);
            return new Weapon(WeaponJTokenFromIndex(index));
        }
        public Weapon GetWeapon(string search)
        {
            if (weaponDataJSON == null)
            {
                LoadWeaponData();
            }
            int index = GetIndex(search);
            return new Weapon(WeaponJTokenFromIndex(index));
        }
    }
}
