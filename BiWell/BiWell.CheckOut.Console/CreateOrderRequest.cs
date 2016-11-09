using System;

namespace BiWell.CheckOut.Console
{
    public class CreateOrderRequest
    {
        public string apiKey { get; set; }
        public OrderRequest order { get; set; }
        public DeliveryRequest delivery { get; set; }
        public UserRequest user { get; set; }
    }

    public class OrderRequest
    {
        public string shopOrderId { get; set; } // Идентификатор заказа в системе ИМ
        public string comment { get; set; }
        public string paymentMethod { get; set; } // тип оплаты cash|nocashpay
        public decimal forcedCost { get; set; }
        public bool forceLabelPrinting { get; set; }
        public ProductRequest[] goods { get; set; }
    }

    public class ProductRequest
    {
        public string name { get; set; } // наименование товара (обязательный)
        public string code { get; set; } // код товара (артикул)
        public string variantCode { get; set; } // код варианта
        public int quantity { get; set; } // количество (обязательный)
        public decimal assessedCost { get; set; } // оценочная стоимость единицы товара (обязательный)
        public decimal payCost { get; set; } // платеж за единицу товара (обязательный)
        public decimal weight { get; set; } // вес (кг) (обязательный)
    }

    public class DeliveryRequest
    {
        public int deliveryId { get; set; } // Идентификатор службы доставки (обязательный)
        public string placeFiasId { get; set; } // Идентификатор населенного пункта по ФИАС
        public string courierOptions { get; set; }
        public string addressPvz { get; set; }
        public string type { get; set; } // mail|express|pvz|postamat
        public decimal cost { get; set; } // стоимость доставки (обязательный)
        public int minTerm { get; set; } // срок доставки - от (обязательный)
        public int maxTerm { get; set; } // срок доставки - до (обязательный)
        public AddressExpressRequest addressExpress { get; set; }
    }

    public class AddressExpressRequest
    {
        public string postIndex { get; set; } // почтовый индекс
        public string streetFiasId { get; set; } // идентификатор улицы по ФИАС
        public string street { get; set; } // наименование улицы
        public string house { get; set; } // дом
        public string housing { get; set; } // корпус
        public string building { get; set; } // строение
        public string appartment { get; set; } // квартира/офис
    }

    public class UserRequest
    {
        public string fullname { get; set; } // (обязательный)
        public string email { get; set; } // (обязательный)
        public string phone { get; set; } // (обязательный)
    }
}
