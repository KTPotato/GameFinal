    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class PlayerData : MonoBehaviour
    {
        public static PlayerData Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<PlayerData>();
                    if (instance == null)
                    {
                        var instanceContainer = new GameObject("PlayerData");
                        instance = instanceContainer.AddComponent<PlayerData>();
                    }
                }
                return instance;
            }
        }

        private static PlayerData instance;

        public List<int> PlayerSkill = new List<int>();

        public void UpdatePlayerStats(string skillType, float value)
        {
            var playerCtrl = FindObjectOfType<Player1Ctrl>();
            if (playerCtrl == null) return;

            switch (skillType)
            {
                case "speed":
                    playerCtrl.speed += value;
                    break;
                case "firespeed":
                    playerCtrl.firespeed -= value; // firespeed는 줄어들수록 빨라짐
                    if (playerCtrl.firespeed < 0.1f) playerCtrl.firespeed = 0.1f; // 최소 값 제한
                    break;
                case "maxHp":
                    playerCtrl.maxHp += value; // 최대 체력 증가
                    playerCtrl.Hp += value;    // 현재 체력도 증가
                    if (playerCtrl.Hp > playerCtrl.maxHp)
                    {
                        playerCtrl.Hp = playerCtrl.maxHp; // 현재 체력을 최대 체력으로 제한
                    }
                    break;
                case "Hp":
                    playerCtrl.Hp += value; // 현재 체력 증가
                    if (playerCtrl.Hp > playerCtrl.maxHp)
                    {
                        playerCtrl.Hp = playerCtrl.maxHp; // 현재 체력을 최대 체력으로 제한
                    }
                    break;
                case "dmg":
                    playerCtrl.dmg += value;
                    break;
                case "crossfireLevel":
                    playerCtrl.crossfireLevel += (int)value;
                    break;
                case "fan_fireLevel":
                    playerCtrl.fan_fireLevel += (int)value;
                    break;
                case "spinballLevel":
                    playerCtrl.spinballLevel += (int)value;
                    break;
            }
        }
    }
