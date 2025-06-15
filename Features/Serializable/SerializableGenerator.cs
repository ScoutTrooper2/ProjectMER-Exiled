using AdminToys;
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
    public class SerializableGenerator : SerializableObject, IIndicatorDefinition
    {
        public KeycardPermissions KeycardPermissions { get; set; }
        [IgnoreToolgunGUI]
        [YamlIgnore]
        public StructurePositionSync StructurePositionSync { get; set; }
        public override GameObject? SpawnOrUpdateObject(Room? room = null, GameObject? instance = null)
        {
            Scp079Generator text = instance == null ? PrefabHelper.Spawn<Scp079Generator>(Exiled.API.Enums.PrefabType.GeneratorStructure) : instance.GetComponent<Scp079Generator>();
            Vector3 position = room.GetAbsolutePosition(Position);
            Quaternion rotation = room.GetAbsoluteRotation(Rotation);
            _prevIndex = Index;

            Generator generator = Generator.Get(text);

            generator.SetPermissionFlag(KeycardPermissions, true);

            generator.Transform.SetPositionAndRotation(position, rotation);
            generator.Transform.localScale = Scale;

            StructurePositionSync = text.GetComponent<StructurePositionSync>();

            text.transform.SetPositionAndRotation(position, rotation);
            text.transform.localScale = Scale;

            StructurePositionSync.Network_position = (text.transform.position);
            StructurePositionSync.Network_rotationY = (sbyte)Mathf.RoundToInt(text.transform.eulerAngles.y / 5.625f);

            NetworkServer.UnSpawn(generator.GameObject);
            NetworkServer.Spawn(generator.GameObject);

            return text.gameObject;
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
    }

}
