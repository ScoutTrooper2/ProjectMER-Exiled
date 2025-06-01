using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Lockers;
using Interactables.Interobjects.DoorUtils;
using MapGeneration.Distributors;
using Mirror;
using ProjectMER.Features.Enums;
using ProjectMER.Features.Extensions;
using ProjectMER.Features.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YamlDotNet.Serialization;
using Locker = Exiled.API.Features.Lockers.Locker;
using LockerType = ProjectMER.Features.Enums.LockerType;

namespace ProjectMER.Features.Serializable
{
    public class SerializableLocker : SerializableObject
    {
        public LockerType LockerType { get; set; } = LockerType.Misc;
        [IgnoreToolgunGUI]
        public Dictionary<int, List<SerializableLockerItem>> Chambers { get; set; } = new()
        {
            { 0, new () { new () } },
        };

        public bool ShuffleChambers { get; set; } = true;

        public KeycardPermissions KeycardPermissions { get; set; } = KeycardPermissions.None;

        public ushort OpenedChambers { get; set; }

        public float Chance { get; set; } = 100f;

        public bool IsSpawnedLoot { get; private set; } = false;
        [IgnoreToolgunGUI]
        [YamlIgnore]
        public StructurePositionSync StructurePositionSync { get; set; }

        public override GameObject SpawnOrUpdateObject(Room? room = null, GameObject? instance = null)
        {
            MapGeneration.Distributors.Locker lockerVariant;
            Vector3 position = room.GetAbsolutePosition(Position);
            Quaternion rotation = room.GetAbsoluteRotation(Rotation);
            _prevIndex = Index;

            if (instance == null)
            {
                lockerVariant = (GameObject.Instantiate<MapGeneration.Distributors.Locker>(LockerPrefab));
            }
            else
            {
                lockerVariant = (instance.gameObject.GetComponent<MapGeneration.Distributors.Locker>());
            }

            Locker = lockerVariant;

            SetupLocker(lockerVariant);

            StructurePositionSync = lockerVariant.GetComponent<StructurePositionSync>();

            lockerVariant.transform.SetPositionAndRotation(position, rotation);
            lockerVariant.transform.localScale = Scale;

            StructurePositionSync.Network_position = (lockerVariant.transform.position);
            StructurePositionSync.Network_rotationY = (sbyte)Mathf.RoundToInt(lockerVariant.transform.eulerAngles.y / 5.625f);


            _prevType = LockerType;

            NetworkServer.UnSpawn(lockerVariant.gameObject);
            NetworkServer.Spawn(lockerVariant.gameObject);

            return lockerVariant.gameObject;
        }


        public void SetupLocker(MapGeneration.Distributors.Locker locker)
        {
            Locker.Loot = Array.Empty<LockerLoot>();
            HandleItems();
            IsSpawnedLoot = true;
        }
        private void HandleItems()
        {
            foreach (LockerChamber lockerChamber in Locker.Chambers)
                lockerChamber.RequiredPermissions = (DoorPermissionFlags)KeycardPermissions;

            Dictionary<int, List<SerializableLockerItem>> chambersCopy = null;
            if (ShuffleChambers)
            {
                chambersCopy = new(Chambers.Count);
                List<List<SerializableLockerItem>> chambersRandomValues = Chambers.Values.OrderBy(x => UnityEngine.Random.value).ToList();
                for (int i = 0; i < Chambers.Count; i++)
                {
                    chambersCopy.Add(i, chambersRandomValues[i]);
                }
            }

            for (int i = 0; i < Locker.Chambers.Length; i++)
            {
                if (i == Chambers.Count)
                    break;

                SerializableLockerItem chosenLoot = Choose(ShuffleChambers ? chambersCopy[i] : Chambers[i]);
                if (chosenLoot == null)
                    continue;

                Locker.Chambers.ElementAt(i).SpawnItem(chosenLoot.Item, (int)chosenLoot.Count);
            }

            Locker.OpenedChambers = OpenedChambers;
        }

        private static SerializableLockerItem Choose(List<SerializableLockerItem> chambers)
        {
            if (chambers == null || chambers.Count == 0)
                return null;

            float total = 0;

            foreach (SerializableLockerItem elem in chambers)
            {
                total += elem.Chance;
            }

            float randomPoint = UnityEngine.Random.value * total;

            for (int i = 0; i < chambers.Count; i++)
            {
                if (randomPoint < chambers[i].Chance)
                {
                    return chambers[i];
                }

                randomPoint -= chambers[i].Chance;
            }

            return chambers[chambers.Count - 1];
        }
        private MapGeneration.Distributors.Locker Locker;

        private MapGeneration.Distributors.Locker LockerPrefab
        {
            get
            {
                MapGeneration.Distributors.Locker prefab = LockerType switch
                {
                    LockerType.RifleRack => PrefabHelper.GetPrefab<MapGeneration.Distributors.Locker>(Exiled.API.Enums.PrefabType.RifleRackStructure),
                    LockerType.LargeGun => PrefabHelper.GetPrefab<MapGeneration.Distributors.Locker>(Exiled.API.Enums.PrefabType.LargeGunLockerStructure),
                    LockerType.Misc => PrefabHelper.GetPrefab<MapGeneration.Distributors.Locker>(Exiled.API.Enums.PrefabType.MiscLocker),
                    LockerType.Medkit => PrefabHelper.GetPrefab<MapGeneration.Distributors.Locker>(Exiled.API.Enums.PrefabType.AdrenalineMedkitStructure),
                    LockerType.Adrenaline => PrefabHelper.GetPrefab<MapGeneration.Distributors.Locker>(Exiled.API.Enums.PrefabType.AdrenalineMedkitStructure),
                    LockerType.ExperementalLocker => PrefabHelper.GetPrefab<MapGeneration.Distributors.Locker>(Exiled.API.Enums.PrefabType.ExperimentalLockerStructure),
                    _ => throw new InvalidOperationException(),
                };

                return prefab;
            }
        }

        public override bool RequiresReloading => LockerType != _prevType || base.RequiresReloading;

        internal LockerType _prevType;
    }

}
