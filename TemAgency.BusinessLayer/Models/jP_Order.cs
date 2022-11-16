using System;
using System.Collections.Generic;

namespace TemAgency.BusinessLayer.Models
{
    public class jP_Order
    {
        public int slipType { get; set; }
        public string slipNumber { get; set; }
        public string orderDate { get; set; }
        public double totalDiscounted { get; set; }
        public double totalVat { get; set; }
        public double totalGross { get; set; }
        public double totalNet { get; set; }
        public int deductionPart1 { get; set; }
        public int deductionPart2 { get; set; }
        public int orgUnitRef { get; set; }
        public ArpRef arpRef { get; set; }
        public ArpOfReceipt arpOfReceipt { get; set; }
        public DepartmentRef departmentRef { get; set; }
        public DivisionRef divisionRef { get; set; }
        public SourceWHRef sourceWHRef { get; set; }
        public Transactions transactions { get; set; }
    }
    public class ArpRef
    {
        public int reference { get; set; }
        public string aRP_Code { get; set; }
    }
    public class ArpOfReceipt
    {
        public int reference { get; set; }
        public string aRPOfReceipt_Code { get; set; }
    }
    public class DepartmentRef
    {
        public int reference { get; set; }
        public DepartmentFirmRef department_FirmRef { get; set; }
        public string department_Code { get; set; }
        public int department_DomainId { get; set; }
    }
    public class DepartmentFirmRef
    {
        public int reference { get; set; }
        public int department_Firm_Companynr { get; set; }
        public int department_Firm_DomainId { get; set; }
    }
    public class DivisionRef
    {
        public int reference { get; set; }
        public DivisionFirmRef division_FirmRef { get; set; }
        public string division_Code { get; set; }
        public int division_DomainId { get; set; }
    }
    public class DivisionFirmRef
    {
        public int reference { get; set; }
        public int division_Firm_Companynr { get; set; }
        public int division_Firm_DomainId { get; set; }
    }
    public class SourceWHRef
    {
        public int reference { get; set; }
        public SourceWHFirmRef sourceWH_FirmRef { get; set; }
        public string sourceWH_Code { get; set; }
        public int sourceWH_DomainId { get; set; }
    }
    public class SourceWHFirmRef
    {
        public int reference { get; set; }
        public int sourceWH_Firm_Companynr { get; set; }
        public int sourceWH_Firm_DomainId { get; set; }
    }
    public class Item
    {
        public int transType { get; set; }
        public int slipOrder { get; set; }
        public int slipType { get; set; }
        public DateTime slipDate { get; set; }
        public int quantity { get; set; }
        public double price { get; set; }
        public double total { get; set; }
        public double vATRate { get; set; }
        public double vATAmount { get; set; }
        public double vATBase { get; set; }
        public double netTotal { get; set; }
        public MasterReference master_Reference { get; set; }
        public OrderSlipRef orderSlipRef { get; set; }
        public ARPRef2 aRPRef { get; set; }
        public DepartmentRef2 departmentRef { get; set; }
        public DivisionRef2 divisionRef { get; set; }
        public SourceWHRef2 sourceWHRef { get; set; }
        public UOMRef uOMRef { get; set; }
    }
    public class Transactions
    {
        public List<Item> items { get; set; }
    }
    public class MasterReference
    {
        public int reference { get; set; }
        public List<ExtraProp> extraProps { get; set; }
    }
    public class ExtraProp
    {
        public string name { get; set; }
        public string value { get; set; }
    }
    public class OrderSlipRef
    {
        public int reference { get; set; }
        public int orderSlip_SlipType { get; set; }
        public string orderSlip_SlipNumber { get; set; }
    }
    public class ARPRef2
    {
        public int reference { get; set; }
        public string aRP_Code { get; set; }
    }
    public class DepartmentRef2
    {
        public int reference { get; set; }
        public DepartmentFirmRef2 department_FirmRef { get; set; }
        public string department_Code { get; set; }
        public int department_DomainId { get; set; }
    }
    public class DepartmentFirmRef2
    {
        public int reference { get; set; }
        public int department_Firm_Companynr { get; set; }
        public int department_Firm_DomainId { get; set; }
    }
    public class DivisionRef2
    {
        public int reference { get; set; }
        public DivisionFirmRef2 division_FirmRef { get; set; }
        public string division_Code { get; set; }
        public int division_DomainId { get; set; }
    }
    public class DivisionFirmRef2
    {
        public int reference { get; set; }
        public int division_Firm_Companynr { get; set; }
        public int division_Firm_DomainId { get; set; }
    }
    public class SourceWHRef2
    {
        public int reference { get; set; }
        public SourceWHFirmRef2 sourceWH_FirmRef { get; set; }
        public string sourceWH_Code { get; set; }
        public int sourceWH_DomainId { get; set; }
    }
    public class SourceWHFirmRef2
    {
        public int reference { get; set; }
        public int sourceWH_Firm_Companynr { get; set; }
        public int sourceWH_Firm_DomainId { get; set; }
    }
    public class UOMRef
    {
        public int reference { get; set; }
        public string uOM_Code { get; set; }
        public UOMUomsetref uOM_Uomsetref { get; set; }
    }
    public class UOMUomsetref
    {
        public int reference { get; set; }
        public string uOM_Uomsetref_Code { get; set; }
    }
}
