using AdminToys;
using Exiled.API.Features;
using Interactables.Interobjects.DoorUtils;
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

namespace ProjectMER.Features.Serializable
{
    public class SerializableClutter : SerializableObject, IIndicatorDefinition
    {
        public ClutterType ClutterType { get; set; } = ClutterType.HCZOpenHallway_Clutter_F;

        public override GameObject SpawnOrUpdateObject(Room? room = null, GameObject? instance = null)
        {
            GameObject clutterObject;
            Vector3 position = room.GetAbsolutePosition(Position);
            Quaternion rotation = room.GetAbsoluteRotation(Rotation);
            _prevIndex = Index;

            if (instance == null)
            {
                clutterObject = GameObject.Instantiate(ClutterPrefab);
            }
            else
            {
                clutterObject = instance;
            }

            clutterObject.transform.SetPositionAndRotation(position, rotation);
            clutterObject.transform.localScale = Scale;

            _prevType = ClutterType;

            NetworkServer.UnSpawn(clutterObject.gameObject);
            NetworkServer.Spawn(clutterObject.gameObject);

            return clutterObject.gameObject;
        }
        public GameObject SpawnOrUpdateIndicator(Room room, GameObject? instance = null)
        {
            PrimitiveObjectToy cube;

            Vector3 position = room.GetAbsolutePosition(Position);
            Quaternion rotation = room.GetAbsoluteRotation(Rotation);

            if (instance == null)
            {
                cube = PrefabHelper.Spawn<PrimitiveObjectToy>(Exiled.API.Enums.PrefabType.PrimitiveObjectToy);
                cube.NetworkPrimitiveType = PrimitiveType.Cube;
                cube.NetworkPrimitiveFlags = PrimitiveFlags.Visible;
                cube.NetworkMaterialColor = new Color(1f, 1f, 1f, 0.9f);
                cube.transform.localScale = Vector3.one * 0.25f;
            }
            else
            {
                cube = instance.GetComponent<PrimitiveObjectToy>();
            }

            cube.transform.SetPositionAndRotation(position, rotation);

            return cube.gameObject;
        }
        private GameObject ClutterPrefab
        {
            get
            {
                GameObject prefab = ClutterType switch
                {
                    ClutterType.HCZOneSided => PrefabHelper.GetPrefab(Exiled.API.Enums.PrefabType.HCZOneSided),
                    ClutterType.HCZTwoSided => PrefabHelper.GetPrefab(Exiled.API.Enums.PrefabType.HCZTwoSided),
                    ClutterType.HCZOpenHallway => PrefabHelper.GetPrefab(Exiled.API.Enums.PrefabType.HCZOpenHallway),
                    ClutterType.HCZOpenHallway_Construct_A => PrefabHelper.GetPrefab(Exiled.API.Enums.PrefabType.HCZOpenHallway_Construct_A),
                    ClutterType.HCZOpenHallway_Clutter_A => PrefabHelper.GetPrefab(Exiled.API.Enums.PrefabType.HCZOpenHallway_Clutter_A),
                    ClutterType.HCZOpenHallway_Clutter_B => PrefabHelper.GetPrefab(Exiled.API.Enums.PrefabType.HCZOpenHallway_Clutter_B),
                    ClutterType.HCZOpenHallway_Clutter_C => PrefabHelper.GetPrefab(Exiled.API.Enums.PrefabType.HCZOpenHallway_Clutter_C),
                    ClutterType.HCZOpenHallway_Clutter_D => PrefabHelper.GetPrefab(Exiled.API.Enums.PrefabType.HCZOpenHallway_Clutter_D),
                    ClutterType.HCZOpenHallway_Clutter_E => PrefabHelper.GetPrefab(Exiled.API.Enums.PrefabType.HCZOpenHallway_Clutter_E),
                    ClutterType.HCZOpenHallway_Clutter_F => PrefabHelper.GetPrefab(Exiled.API.Enums.PrefabType.HCZOpenHallway_Clutter_F),
                    ClutterType.HCZOpenHallway_Clutter_G => PrefabHelper.GetPrefab(Exiled.API.Enums.PrefabType.HCZOpenHallway_Clutter_G),
                    _ => throw new InvalidOperationException(),
                };

                return prefab;
            }
        }

        public override bool RequiresReloading => ClutterType != _prevType || base.RequiresReloading;

        internal ClutterType _prevType;
    }

}
