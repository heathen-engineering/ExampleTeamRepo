#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using Steamworks;
using System;
using UnityEngine;

namespace HeathenEngineering.SteamworksIntegration
{
    [Serializable]
    public struct InputActionSetData : IEquatable<InputActionSetHandle_t>, IComparable<InputActionSetHandle_t>, IEquatable<ulong>, IComparable<ulong>
    {
        [SerializeField]
        private InputActionSetHandle_t handle;

        public ulong Handle
        {
            get => handle.m_InputActionSetHandle;
            set => handle = new InputActionSetHandle_t(value);
        }

        public bool IsActive(InputControllerData controller) => IsActive(controller.handle);
        public bool IsActive(Steamworks.InputHandle_t controller)
        {
            if (handle.m_InputActionSetHandle != 0)
            {
                var layers = API.Input.Client.GetCurrentActionSet(controller);
                if (layers.m_InputActionSetHandle == handle.m_InputActionSetHandle)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public void Activate(InputControllerData controller) => Activate(controller.handle);
        public void Activate(Steamworks.InputHandle_t controller)
        {
            if (handle.m_InputActionSetHandle != 0)
            {
                API.Input.Client.ActivateActionSet(controller, handle);
            }
        }

        public static InputActionSetData Get(string setName)
        {
            return new InputActionSetData
            {
                handle = API.Input.Client.GetActionSetHandle(setName)
            };
        }

        public static InputActionSetData Get(InputActionSetHandle_t handle)
        {
            return new InputActionSetData
            {
                handle = handle,
            };
        }

        public static InputActionSetData Get(ulong handleValue)
        {
            return new InputActionSetData
            {
                handle = new InputActionSetHandle_t(handleValue),
            };
        }

        public bool Equals(InputActionSetHandle_t other)
        {
            return handle.Equals(other);
        }

        public int CompareTo(InputActionSetHandle_t other)
        {
            return handle.CompareTo(other);
        }

        public bool Equals(ulong other)
        {
            return handle.m_InputActionSetHandle.Equals(other);
        }

        public int CompareTo(ulong other)
        {
            return handle.m_InputActionSetHandle.CompareTo(other);
        }

        public override bool Equals(object obj)
        {
            return handle.Equals(obj);
        }

        public override int GetHashCode()
        {
            return handle.GetHashCode();
        }

        public static bool operator ==(InputActionSetData l, InputActionSetHandle_t r) => l.handle == r;
        public static bool operator ==(InputActionSetData l, ulong r) => l.handle == new InputActionSetHandle_t(r);
        public static bool operator ==(InputActionSetHandle_t l, InputActionSetData r) => l == r.handle;
        public static bool operator ==(ulong l, InputActionSetData r) => new InputActionSetHandle_t(l) == r.handle;
        public static bool operator !=(InputActionSetData l, InputActionSetHandle_t r) => l.handle != r;
        public static bool operator !=(InputActionSetData l, ulong r) => l.handle != new InputActionSetHandle_t(r);
        public static bool operator !=(InputActionSetHandle_t l, InputActionSetData r) => l != r.handle;
        public static bool operator !=(ulong l, InputActionSetData r) => new InputActionSetHandle_t(l) != r.handle;

        public static implicit operator ulong(InputActionSetData c) => c.handle.m_InputActionSetHandle;
        public static implicit operator InputActionSetData(ulong id) => Get(id);
        public static implicit operator InputActionSetHandle_t(InputActionSetData c) => c.handle;
        public static implicit operator InputActionSetData(InputActionSetHandle_t id) => Get(id);
    }
}
#endif