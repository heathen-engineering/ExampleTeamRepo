#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using Steamworks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace HeathenEngineering.SteamworksIntegration
{
    [Serializable]
    public struct ItemData : IEquatable<ItemData>, IEquatable<int>, IEquatable<SteamItemDef_t>, IComparable<ItemData>, IComparable<int>, IComparable<SteamItemDef_t>
    {
        public int id;

        [NonSerialized]
        private ItemDefinitionObject _so;
        public ItemDefinitionObject ScriptableObject
        {
            get
            {
                if (SteamSettings.current == null)
                    return null;

                if (_so == null)
                {
                    var nId = this;
                    _so = SteamSettings.Client.inventory.items.FirstOrDefault(p => p.id == nId);
                }

                return _so;
            }
            set
            {
                _so = value;
                id = value.id;
            }
        }

        public string Name => API.Inventory.Client.GetItemDefinitionProperty(new SteamItemDef_t(id), "name");
        public bool HasPrice => API.Inventory.Client.GetItemPrice(new SteamItemDef_t(id), out ulong _, out ulong _);
        public static Currency.Code CurrencyCode => API.Inventory.Client.LocalCurrencyCode;
        public static string CurrencySymbol => API.Inventory.Client.LocalCurrencySymbol;
        public ulong CurrentPrice
        {
            get
            {
                if (API.Inventory.Client.GetItemPrice(new SteamItemDef_t(id), out ulong current, out ulong _))
                {
                    return current;
                }
                else
                    return 0;
            }
        }
        public ulong BasePrice
        {
            get
            {
                if (API.Inventory.Client.GetItemPrice(new SteamItemDef_t(id), out ulong _, out ulong baseprice))
                {
                    return baseprice;
                }
                else
                    return 0;
            }
        }
        public List<ItemDetail> GetDetails() => API.Inventory.Client.Details(this);
        public long GetTotalQuantity() => API.Inventory.Client.ItemTotalQuantity(this);
        public bool AddPromoItem(Action<InventoryResult> callback) => API.Inventory.Client.AddPromoItem(new SteamItemDef_t(id), callback);
        public ConsumeOrder[] GetConsumeOrders(uint quantity)
        {
            var details = GetDetails();

            if (details.Sum(p => (long)p.Quantity) < quantity)
                return null;

            var results = new List<ConsumeOrder>();
            uint count = 0;
            var index = 0;
            while (count < quantity)
            {
                uint cCount = (uint)Mathf.Min(details[index].Quantity, quantity - count);
                count += cCount;

                results.Add(new ConsumeOrder
                {
                    detail = details[index].itemDetails,
                    quantity = cCount
                });
            }

            return results.ToArray();
        }
        public bool Consume(Action<InventoryResult> callback)
        {
            var details = GetDetails();

            if (details.Sum(p => (long)p.Quantity) < 1)
                return false;

            var instance = details.First(p => p.Quantity > 0);
            API.Inventory.Client.ConsumeItem(instance.itemDetails.m_itemId, 1, callback);
            return true;
        }
        public void Consume(ConsumeOrder order, Action<InventoryResult> callback) => API.Inventory.Client.ConsumeItem(order.detail.m_itemId, order.quantity, callback);
        public bool Consume(uint quantity, Action<InventoryResult> callback)
        {
            var orders = GetConsumeOrders(quantity);
            if(orders == null || orders.Length < 1)
            {
                return false;
            }
            else
            {
                List<ItemDetail> details = new List<ItemDetail>();
                EResult eResult = EResult.k_EResultOK;
                var worker = new BackgroundWorker();
                worker.DoWork += (sender, eventArgs) =>
                {
                    foreach(var order in orders)
                    {
                        bool wait = true;
                        
                        API.Inventory.Client.ConsumeItem(order.detail.m_itemId, order.quantity, (r) =>
                        {
                            eResult = r.result;

                            if (eResult == EResult.k_EResultOK)
                                details.AddRange(r.items);
                                
                            wait = false;
                        });

                        while(wait)
                            Thread.Sleep(50);

                        if (eResult != EResult.k_EResultOK)
                            break;
                    }

                    var final = new InventoryResult();
                    final.result = eResult;
                    final.items = details.ToArray();

                    eventArgs.Result = final;
                };
                worker.RunWorkerCompleted += (sender, eventArgs) =>
                {
                    var final = (InventoryResult)eventArgs.Result;
                    callback?.Invoke(final);
                    worker.Dispose();
                };
                return true;
            }
        }
        public bool GetExchangeEntry(uint quantity, out ExchangeEntry[] entries)
        {
            var details = GetDetails();

            if (details.Sum(p => (long)p.Quantity) < quantity)
            {
                entries = new ExchangeEntry[0];
                return false;
            }
            else
            {
                var list = new List<ExchangeEntry>();
                uint count = 0;
                var index = 0;
                while (count < quantity)
                {
                    if (details[index].Quantity <= quantity - count)
                    {
                        if (details[index].Quantity > 0)
                        {
                            list.Add(new ExchangeEntry
                            {
                                instance = details[index].ItemId,
                                quantity = details[index].Quantity
                            });
                            count += details[index].Quantity;
                        }
                    }
                    else
                    {
                        if (details[index].Quantity > 0)
                        {
                            var remainder = (uint)(quantity - count);
                            list.Add(new ExchangeEntry
                            {
                                instance = details[index].ItemId,
                                quantity = remainder,
                            });
                            count += remainder;
                        }
                    }

                    index++;
                }

                entries = list.ToArray();
                return true;
            }
        }
        public void Exchange(IEnumerable<ExchangeEntry> recipeEntries, Action<InventoryResult> callback)
        {
            var list = recipeEntries.ToArray();

            var instances = new SteamItemInstanceID_t[list.Length];
            var counts = new uint[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                instances[i] = list[i].instance;
                counts[i] = list[i].quantity;
            }
            API.Inventory.Client.ExchangeItems(new SteamItemDef_t(id), instances, counts, callback);
        }
        public void GenerateItem(Action<InventoryResult> callback)
        {
            API.Inventory.Client.GenerateItems(new SteamItemDef_t[] { new SteamItemDef_t(id) }, new uint[] { 1 }, callback);
        }
        public void GenerateItem(uint quantity, Action<InventoryResult> callback)
        {
            API.Inventory.Client.GenerateItems(new SteamItemDef_t[] { new SteamItemDef_t(id) }, new uint[] { quantity }, callback);
        }
        public void StartPurchase(Action<SteamInventoryStartPurchaseResult_t, bool> callback)
        {
            API.Inventory.Client.StartPurchase(new SteamItemDef_t[] { new SteamItemDef_t(id) }, new uint[] { 1 }, callback);
        }
        public void StartPurchase(uint count, Action<SteamInventoryStartPurchaseResult_t, bool> callback)
        {
            API.Inventory.Client.StartPurchase(new SteamItemDef_t[] { new SteamItemDef_t(id) }, new uint[] { count }, callback);
        }
        public bool GetPrice(out ulong currentPrice, out ulong basePrice) => API.Inventory.Client.GetItemPrice(new SteamItemDef_t(id), out currentPrice, out basePrice);
        public void TriggerDrop(Action<InventoryResult> callback) => API.Inventory.Client.TriggerItemDrop(new SteamItemDef_t(id), callback);
        public string CurrentPriceString()
        {
            System.Globalization.NumberFormatInfo cultureInfo = (System.Globalization.NumberFormatInfo)System.Globalization.CultureInfo.CurrentCulture.NumberFormat.Clone();
            cultureInfo.CurrencySymbol = CurrencySymbol;

            return ((double)CurrentPrice / 100).ToString("c", cultureInfo);
        }
        public string BasePriceString()
        {
            System.Globalization.NumberFormatInfo cultureInfo = (System.Globalization.NumberFormatInfo)System.Globalization.CultureInfo.CurrentCulture.NumberFormat.Clone();
            cultureInfo.CurrencySymbol = CurrencySymbol;

            return ((double)BasePrice / 100).ToString("c", cultureInfo);
        }

        public static void RequestPrices(Action<SteamInventoryRequestPricesResult_t, bool> callback) => API.Inventory.Client.RequestPrices(callback);
        public static void Update(Action<InventoryResult> callback) => API.Inventory.Client.GetAllItems(callback);
        public static ItemData Get(int id) => id;
        public static ItemData Get(SteamItemDef_t id) => id;
        public static ItemData Get(ItemDefinitionObject item) => item.id;

        #region Boilerplate
        public int CompareTo(ItemData other)
        {
            return id.CompareTo(other.id);
        }

        public int CompareTo(int other)
        {
            return id.CompareTo(other);
        }

        public int CompareTo(SteamItemDef_t other)
        {
            return id.CompareTo(other);
        }

        public bool Equals(ItemData other)
        {
            return id.Equals(other.id);
        }

        public bool Equals(int other)
        {
            return id.Equals(other);
        }

        public bool Equals(SteamItemDef_t other)
        {
            return id.Equals(other);
        }

        public override bool Equals(object obj)
        {
            return id.Equals(obj);
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public static bool operator ==(ItemData l, ItemData r) => l.id == r.id;
        public static bool operator ==(ItemData l, int r) => l.id == r;
        public static bool operator ==(ItemData l, SteamItemDef_t r) => l.id == r.m_SteamItemDef;
        public static bool operator !=(ItemData l, ItemData r) => l.id != r.id;
        public static bool operator !=(ItemData l, int r) => l.id != r;
        public static bool operator !=(ItemData l, SteamItemDef_t r) => l.id != r.m_SteamItemDef;

        public static implicit operator int(ItemData c) => c.id;
        public static implicit operator ItemData(int id) => new ItemData { id = id };
        public static implicit operator SteamItemDef_t(ItemData c) => new SteamItemDef_t(c.id);
        public static implicit operator ItemData(SteamItemDef_t id) => new ItemData { id = id.m_SteamItemDef };
        #endregion
    }
}
#endif