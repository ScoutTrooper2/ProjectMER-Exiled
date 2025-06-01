using Exiled.API.Enums;
using Exiled.API.Features;
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

namespace ProjectMER.Features.Serializable
{
    public class SerializablePedestalScp : SerializableObject
    {
        public PedestalType PedestalType { get; set; } = PedestalType.Scp207;

        public ItemType ItemContainer { get; set; } = ItemType.SCP207;

        public bool ShuffleChambers { get; set; } = true;

        public KeycardPermissions KeycardPermissions { get; set; } = KeycardPermissions.None;
        [IgnoreToolgunGUI]
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
                lockerVariant = (GameObject.Instantiate<MapGeneration.Distributors.Locker>(PedestalPrefab));
            }
            else
            {
                lockerVariant = (instance.gameObject.GetComponent<MapGeneration.Distributors.Locker>());
            }

            Pedestal = lockerVariant;

            SetupLocker(lockerVariant);

            StructurePositionSync = lockerVariant.GetComponent<StructurePositionSync>();

            lockerVariant.transform.SetPositionAndRotation(position, rotation);
            lockerVariant.transform.localScale = Scale;

            StructurePositionSync.Network_position = (lockerVariant.transform.position);
            StructurePositionSync.Network_rotationY = (sbyte)Mathf.RoundToInt(lockerVariant.transform.eulerAngles.y / 5.625f);

            _prevType = PedestalType;

            NetworkServer.UnSpawn(lockerVariant.gameObject);
            NetworkServer.Spawn(lockerVariant.gameObject);

            return lockerVariant.gameObject;
        }


        public void SetupLocker(MapGeneration.Distributors.Locker locker)
        {
            Pedestal.Loot = Array.Empty<LockerLoot>();
            HandleItems();
            IsSpawnedLoot = true;
        }
        private void HandleItems()
        {
            foreach (LockerChamber lockerChamber in Pedestal.Chambers)
                lockerChamber.RequiredPermissions = (DoorPermissionFlags)KeycardPermissions;

            for (int i = 0; i < Pedestal.Chambers.Length; i++)
            {
                Pedestal.Chambers.ElementAt(i).SpawnItem(ItemContainer, 1);
                break;
            }
        }


        private MapGeneration.Distributors.Locker Pedestal;

        private MapGeneration.Distributors.Locker PedestalPrefab
        {
            get
            {
                MapGeneration.Distributors.Locker prefab = PedestalType switch
                {
                    PedestalType.Scp018 => PrefabHelper.GetPrefab<MapGeneration.Distributors.Locker>(Exiled.API.Enums.PrefabType.Scp018PedestalStructure),
                    PedestalType.Scp1853 => PrefabHelper.GetPrefab<MapGeneration.Distributors.Locker>(Exiled.API.Enums.PrefabType.Scp1853PedestalStructure),
                    PedestalType.Scp244 => PrefabHelper.GetPrefab<MapGeneration.Distributors.Locker>(Exiled.API.Enums.PrefabType.Scp244PedestalStructure),
                    PedestalType.Scp268 => PrefabHelper.GetPrefab<MapGeneration.Distributors.Locker>(Exiled.API.Enums.PrefabType.Scp268PedestalStructure),
                    PedestalType.Scp2176 => PrefabHelper.GetPrefab<MapGeneration.Distributors.Locker>(Exiled.API.Enums.PrefabType.Scp2176PedestalStructure),
                    PedestalType.Scp1576 => PrefabHelper.GetPrefab<MapGeneration.Distributors.Locker>(Exiled.API.Enums.PrefabType.Scp1576PedestalStructure),
                    PedestalType.Scp500 => PrefabHelper.GetPrefab<MapGeneration.Distributors.Locker>(Exiled.API.Enums.PrefabType.Scp500PedestalStructure),
                    PedestalType.Scp1344 => PrefabHelper.GetPrefab<MapGeneration.Distributors.Locker>(Exiled.API.Enums.PrefabType.Scp1344PedestalStructure),
                    PedestalType.AntiScp207 => PrefabHelper.GetPrefab<MapGeneration.Distributors.Locker>(Exiled.API.Enums.PrefabType.AntiScp207PedestalStructure),
                    PedestalType.Scp207 => PrefabHelper.GetPrefab<MapGeneration.Distributors.Locker>(Exiled.API.Enums.PrefabType.Scp207PedestalStructure),
                    _ => throw new InvalidOperationException(),
                };

                return prefab;
            }
        }

        public override bool RequiresReloading => PedestalType != _prevType || base.RequiresReloading;

        internal PedestalType _prevType;
    }
}
