using CodeStage.AntiCheat.ObscuredTypes;
using SimpleJSON;
using System;
using UnityEngine;

public class EnvironmentConfig
{
    private static JSONObject _config = null;
    private static JSONObject config
    {
        get
        {
            if (_config == null)
            {
                _config = JSON.Parse(Resources.Load<TextAsset>("Config/EnvironmentConfig").text).AsObject;
            }
            return _config;
        }
    }

    private static ObscuredString _current_environment = "";
    private static ObscuredString current_environment
    {
        get
        {
            if (string.IsNullOrEmpty(_current_environment))
            {
                _current_environment = config["environment"].ToString().Replace("\"", "");
            }
            return _current_environment;
        }
    }
    private static EnvironmentType _current_environment_enum = EnvironmentType.none;
    public static EnvironmentType currentEnvironmentEnum
    {
        get
        {
            if (_current_environment_enum == EnvironmentType.none)
            {
                _current_environment_enum = Enum.Parse<EnvironmentType>(current_environment);
            }
            return _current_environment_enum;
        }
    }

    private static ObscuredString _ad_banner_id_aos = "";
    public static ObscuredString adBannerIdAOS
    {
        get
        {
            if (string.IsNullOrEmpty(_ad_banner_id_aos))
            {
                string ad_id = config[current_environment]["ad_banner_id_aos"].ToString().Replace("\"", "");
                _ad_banner_id_aos = ad_id;
            }
            return _ad_banner_id_aos;
        }
    }

    private static ObscuredString _ad_banner_id_ios = "";
    public static ObscuredString adBannerIdIOS
    {
        get
        {
            if (string.IsNullOrEmpty(_ad_banner_id_ios))
            {
                string ad_id = config[current_environment]["ad_banner_id_ios"].ToString().Replace("\"", "");
                _ad_banner_id_ios = ad_id;
            }
            return _ad_banner_id_ios;
        }
    }

    private static ObscuredString _ad_inter_id_aos = "";
    public static ObscuredString adInterIdAOS
    {
        get
        {
            if (string.IsNullOrEmpty(_ad_inter_id_aos))
            {
                string ad_id = config[current_environment]["ad_inter_id_aos"].ToString().Replace("\"", "");
                _ad_inter_id_aos = ad_id;
            }
            return _ad_inter_id_aos;
        }
    }

    private static ObscuredString _ad_inter_id_ios = "";
    public static ObscuredString adInterIdIOS
    {
        get
        {
            if (string.IsNullOrEmpty(_ad_inter_id_ios))
            {
                string ad_id = config[current_environment]["ad_inter_id_ios"].ToString().Replace("\"", "");
                _ad_inter_id_ios = ad_id;
            }
            return _ad_inter_id_ios;
        }
    }

    private static ObscuredString _ad_reward_id_aos = "";
    public static ObscuredString adRewardIdAOS
    {
        get
        {
            if (string.IsNullOrEmpty(_ad_reward_id_aos))
            {
                string ad_id = config[current_environment]["ad_reward_id_aos"].ToString().Replace("\"", "");
                _ad_reward_id_aos = ad_id;
            }
            return _ad_reward_id_aos;
        }
    }

    private static ObscuredString _ad_reward_id_ios = "";
    public static ObscuredString adRewardIdIOS
    {
        get
        {
            if (string.IsNullOrEmpty(_ad_reward_id_ios))
            {
                string ad_id = config[current_environment]["ad_reward_id_ios"].ToString().Replace("\"", "");
                _ad_reward_id_ios = ad_id;
            }
            return _ad_reward_id_ios;
        }
    }

    private static ObscuredString _ad_inter_reward_id_aos = "";
    public static ObscuredString adInterRewardIdAOS
    {
        get
        {
            if (string.IsNullOrEmpty(_ad_inter_reward_id_aos))
            {
                string ad_id = config[current_environment]["ad_inter_reward_id_aos"].ToString().Replace("\"", "");
                _ad_inter_reward_id_aos = ad_id;
            }
            return _ad_inter_reward_id_aos;
        }
    }

    private static ObscuredString _ad_inter_reward_id_ios = "";
    public static ObscuredString adInterRewardIdIOS
    {
        get
        {
            if (string.IsNullOrEmpty(_ad_inter_reward_id_ios))
            {
                string ad_id = config[current_environment]["ad_inter_reward_id_ios"].ToString().Replace("\"", "");
                _ad_inter_reward_id_ios = ad_id;
            }
            return _ad_inter_reward_id_ios;
        }
    }
    private static ObscuredString _link_game_store = "";
    public static ObscuredString linkGameStore
    {
        get
        {
            if (string.IsNullOrEmpty(_link_game_store))
            {
                string ad_id = config[current_environment]["link_game_store"].ToString().Replace("\"", "");
                _link_game_store = ad_id;
            }
            return _link_game_store;
        }
    }

    private static JSONObject _dataVersion = null;
    private static ObscuredString _version = "";
    public static ObscuredString version
    {
        get
        {
            if (_dataVersion == null)
            {
                _dataVersion = JSON.Parse(Resources.Load<TextAsset>("Data/Version/Version").text).AsObject;
            }
            if (string.IsNullOrEmpty(_version))
            {
                string _versionjs = _dataVersion["version"].ToString().Replace("\"", "");
                _version = _versionjs;
            }
            return _version;
        }
    }

    private static ObscuredString _ad_banner_id_applovin_aos = "";
    public static ObscuredString adBannerIdApplovinAOS
    {
        get
        {
            if (string.IsNullOrEmpty(_ad_banner_id_applovin_aos))
            {
                string ad_id = config[current_environment]["ad_banner_id_applovin_aos"].ToString().Replace("\"", "");
                _ad_banner_id_applovin_aos = ad_id;
            }
            return _ad_banner_id_applovin_aos;
        }
    }

    private static ObscuredString _ad_inter_id_applovin_aos = "";
    public static ObscuredString adInterIdApplovinAOS
    {
        get
        {
            if (string.IsNullOrEmpty(_ad_inter_id_applovin_aos))
            {
                string ad_id = config[current_environment]["ad_inter_id_applovin_aos"].ToString().Replace("\"", "");
                _ad_inter_id_applovin_aos = ad_id;
            }
            return _ad_inter_id_applovin_aos;
        }
    }

    private static ObscuredString _ad_reward_id_applovin_aos = "";
    public static ObscuredString adRewardIdApplovinAOS
    {
        get
        {
            if (string.IsNullOrEmpty(_ad_reward_id_applovin_aos))
            {
                string ad_id = config[current_environment]["ad_reward_id_applovin_aos"].ToString().Replace("\"", "");
                _ad_reward_id_applovin_aos = ad_id;
            }
            return _ad_reward_id_applovin_aos;
        }
    }
}
