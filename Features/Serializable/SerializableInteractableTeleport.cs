using AdminToys;
using Exiled.API.Features;
using LabApi.Features.Wrappers;
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
using static AdminToys.InvisibleInteractableToy;
using PrimitiveObjectToy = AdminToys.PrimitiveObjectToy;
using Room = Exiled.API.Features.Room;

namespace ProjectMER.Features.Serializable
{
    public class SerializableInteractableTeleport : SerializableObject, IIndicatorDefinition
    {
        public string ToTeleportID { get; set; } = "PutID";
        public string ThisTeleportID { get; set; } = "PutID";
        public float InteractionDuration { get; set; } = 1;
        public bool IsLocked { get; set; } = false;
        public override GameObject SpawnOrUpdateObject(Room? room = null, GameObject? instance = null)
        {
            InvisibleInteractableToy interactabletoy = instance == null ? UnityEngine.Object.Instantiate(TargetPrefab) : instance.GetComponent<InvisibleInteractableToy>();
            Vector3 position = room.GetAbsolutePosition(Position);
            Quaternion rotation = room.GetAbsoluteRotation(Rotation);

            interactabletoy.transform.SetPositionAndRotation(position, rotation);
            interactabletoy.transform.localScale = Scale;

            InteractableToy.Get(interactabletoy);

            interactabletoy.NetworkInteractionDuration = InteractionDuration;
            interactabletoy.NetworkIsLocked = IsLocked;
            interactabletoy.NetworkScale = Scale;
            interactabletoy.NetworkPosition = interactabletoy.transform.position;
            interactabletoy.NetworkRotation = interactabletoy.transform.rotation;

            if (instance == null)
                NetworkServer.Spawn(interactabletoy.gameObject);

            return interactabletoy.gameObject;
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
                cube.NetworkScale = Scale;
            }
            else
            {
                cube = instance.GetComponent<PrimitiveObjectToy>();
            }

            cube.transform.SetPositionAndRotation(position, rotation);

            return cube.gameObject;
        }
        private AdminToys.InvisibleInteractableToy TargetPrefab
        {
            get
            {
                return PrefabHelper.GetPrefab<InvisibleInteractableToy>(Exiled.API.Enums.PrefabType.InvisibleInteractableToy);
            }
        }

        public override bool RequiresReloading => base.RequiresReloading;
    }
}