using Dignus.DependencyInjection.Attributes;
using Dignus.Log;
using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;


namespace Assets.Scripts.Service
{
    public record UserModel
    {
        public string AccountId { get; set; }

        public string Nickname { get; set; }
    }

    [Injectable(Dignus.DependencyInjection.LifeScope.Singleton)]
    public class UserService
    {
        private readonly string _savePath = Path.Combine(Application.persistentDataPath, $"UserData.dat");

        private UserModel _userModel;

        public string CreateAccountId()
        {
            return $"{SystemInfo.deviceUniqueIdentifier}-{DateTime.Now.Ticks}";
        }
        public UserModel GetUserModel()
        {
            return _userModel;
        }
        public void SetUserModel(UserModel userModel)
        {
            _userModel = userModel;
        }
        public bool Load()
        {
            if (File.Exists(_savePath) == false)
            {
                return false;
            }

            var data = File.ReadAllText(_savePath);
            try
            {
                _userModel = JsonConvert.DeserializeObject<UserModel>(data);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return false;
            }
            return true;
        }

        public void SaveData()
        {
            var json = JsonConvert.SerializeObject(_userModel);
            File.WriteAllText(_savePath, json);
        }
    }
}
