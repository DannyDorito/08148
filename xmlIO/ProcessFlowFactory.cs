using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using logic;

namespace xmlIO
{
    public class ProcessFlowFactory
    {
        private const String outFileName = "out.xml";
        public static List<IFlowOperation> LoadInput(String pathname)
        {
            XmlDocument flowinXml = new XmlDocument();
            //try
            //{
            using (TextReader reader = File.OpenText(pathname))
            {
                flowinXml.Load(reader);
            }
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //}
            XmlNodeList inputsXml = flowinXml.SelectNodes("input")[0].ChildNodes;
            List<IFlowOperation> result = new List<IFlowOperation>();
            foreach (XmlNode inputXml in inputsXml)
            {
                switch (inputXml.Name)
                {
                    case "flow":
                        {
                            ProcessFlow processFlow = MkProcessFlow(inputXml);
                            result.Add(processFlow);
                            break;
                        }
                    case "execute":
                        {
                            //try
                            //{
                            result.Add(new OpExecute(Int32.Parse(inputXml.InnerText)));
                            //}
                            //catch (Exception e)
                            //{
                            //    // do nothing
                            //}
                            break;
                        }
                    case "query":
                        {
                            XmlNodeList storesXml = inputXml.SelectNodes("stores/store");
                            List<String> stores = new List<String>();
                            foreach (XmlNode storeXml in storesXml)
                            {
                                stores.Add(storeXml.InnerText);
                            }
                            result.Add(new OpQuery(stores));
                            break;
                        }
                    case "load":
                        {
                            XmlNodeList storeAmountsXml = inputXml.SelectNodes("storeAmount");
                            List<StoreIDAmount> storeAmounts = new List<StoreIDAmount>();
                            foreach (XmlNode storeAmountXml in storeAmountsXml)
                            {
                                XmlNodeList storeXml = storeAmountXml.SelectNodes("store");
                                XmlNodeList amountXml = storeAmountXml.SelectNodes("amount");
                                storeAmounts.Add(new StoreIDAmount(storeXml[0].InnerText, Int32.Parse(amountXml[0].InnerText)));
                            }
                            result.Add(new OpLoad(storeAmounts));
                            break;
                        }
                    case "#comment":
                        {
                            break;
                        }
                    default:
                        {
                            throw new Exception("Unknown Xml: " + inputXml.Name);
                        }
                }
            }
            return result;
        }

        public static List<int> LoadOutput(String pathname)
        {

            XmlDocument flowoutXml = new XmlDocument();
            //try
            //{
            using (TextReader reader = File.OpenText(pathname))
            {
                flowoutXml.Load(reader);
            }
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //}
            XmlNodeList outputsXml = flowoutXml.SelectNodes("OpQueryOutput")[0].ChildNodes;
            List<int> result = new List<int>();
            foreach (XmlNode outputXml in outputsXml)
            {
                switch (outputXml.Name)
                {
                    case "amounts":
                        {
                            XmlNodeList amountsXml = outputXml.SelectNodes("int");
                            foreach (XmlNode storeXml in amountsXml)
                            {
                                result.Add(Int32.Parse(storeXml.InnerText));
                            }
                            break;
                        }
                    default:
                        {
                            throw new Exception("Unknown Xml: " + outputXml.Name);
                        }
                }
            }
            return result;
        }

        public static Store MkStore(XmlNode storeXml)
        {
            XmlNodeList idXml = storeXml.SelectNodes("id");
            String id = idXml[0].InnerText;
            XmlNodeList typXml = storeXml.SelectNodes("typ");
            String typ = typXml[0].InnerText;
            XmlNodeList amountXml = storeXml.SelectNodes("amount");
            int amount = Int32.Parse(amountXml[0].InnerText);
            // optional fields
            XmlNodeList capacityXml = storeXml.SelectNodes("capacity");
            String capacityData = null;
            if (capacityXml.Count > 0)
            {
                capacityData = capacityXml[0].InnerText;
            }
            if (capacityData != null)
            {
                return new Store(id, typ, amount, Int32.Parse(capacityData));
            }
            return new Store(id, typ, amount);
        }

        public static Stores MkStores(XmlNode storesXml)
        {
            List<Store> stores = new List<Store>();
            XmlNodeList storeXml = storesXml.SelectNodes("store");
            foreach (XmlNode storXml in storeXml)
            {
                stores.Add(MkStore(storXml));
            }
            return new Stores(stores);
        }

        public static Process MkProcess(XmlNode processXml)
        {
            XmlNodeList idXml = processXml.SelectNodes("id");
            String id = idXml[0].InnerText;
            List<String> typsIn = new List<String>();
            XmlNodeList typsInXml = processXml.SelectNodes("typsIn");
            foreach (XmlNode typInXml in typsInXml)
            {
                typsIn.Add(typInXml.FirstChild.InnerText);
            }
            List<String> typsOut = new List<String>();
            XmlNodeList typsOutXml = processXml.SelectNodes("typsOut");
            foreach (XmlNode typOutXml in typsOutXml)
            {
                typsOut.Add(typOutXml.FirstChild.InnerText);
            }
            return new Process(id, typsIn, typsOut);
        }

        public static Processes MkProcesses(XmlNode processesXml)
        {
            List<Process> processes = new List<Process>();
            XmlNodeList procsXml = processesXml.SelectNodes("process");
            foreach (XmlNode procXml in procsXml)
            {
                processes.Add(MkProcess(procXml));
            }
            return new Processes(processes);
        }

        public static Links MkLinks(XmlNode linksXml, Stores stores, Processes processes)
        {
            List<Link> links = new List<Link>();
            XmlNodeList lnksXml = linksXml.SelectNodes("linkin");
            foreach (XmlNode lnkXml in lnksXml)
            {
                LinkIn l = MkLinkIn(lnkXml);
                links.Add(l);
            }
            lnksXml = linksXml.SelectNodes("linkout");
            foreach (XmlNode lnkXml in lnksXml)
            {
                LinkOut l = MkLinkOut(lnkXml);
                links.Add(l);
            }
            return new Links(links);
        }

        public static LinkIn MkLinkIn(XmlNode node)
        {
            String id = node.SelectNodes("id")[0].InnerText;
            String sourceId = node.SelectNodes("source")[0].InnerText;
            String targetId = node.SelectNodes("target")[0].InnerText;
            int volume = Int32.Parse(node.SelectNodes("amount")[0].InnerText);
            return new LinkIn(id, sourceId, targetId, volume);
        }

        public static LinkOut MkLinkOut(XmlNode node)
        {
            String id = node.SelectNodes("id")[0].InnerText;
            String sourceId = node.SelectNodes("source")[0].InnerText;
            String targetId = node.SelectNodes("target")[0].InnerText;
            int volume = Int32.Parse(node.SelectNodes("amount")[0].InnerText);
            return new LinkOut(id, sourceId, targetId, volume);
        }

        public static ProcessFlow MkProcessFlow(XmlNode flowXml)
        {
            if (XmlFormat(flowXml))
            {
                XmlNodeList storesXml = flowXml.SelectNodes("stores");
                Stores stores = MkStores(storesXml[0]);
                XmlNodeList processesXml = flowXml.SelectNodes("processes");
                Processes processes = MkProcesses(processesXml[0]);
                XmlNodeList linksXml = flowXml.SelectNodes("links");
                Links links = MkLinks(linksXml[0], stores, processes);
                return new ProcessFlow(stores, processes, links);
            }
            else
            {
                return null;
            }
        }

        public static bool XmlFormat(XmlNode flowXml)
        {
            foreach (XmlNode flowItem in flowXml.ChildNodes)
            {
                foreach (XmlNode flowChild in flowItem)
                {
                    if (flowChild.Name == "store")
                    {
                        if (flowChild.SelectNodes("id").Count > 1 || flowChild.SelectNodes("id").Count <= 0)
                        {
                            return true;
                        }
                        if (flowChild.SelectNodes("typ").Count > 1 || flowChild.SelectNodes("typ").Count <= 0)
                        {
                            return true;
                        }
                        if (flowChild.SelectNodes("amount").Count > 1 || flowChild.SelectNodes("amount").Count <= 0)
                        {
                            return true;
                        }
                        if (flowChild.SelectNodes("capacity").Count > 1 || flowChild.SelectNodes("capacity").Count <= 0)
                        {
                            return true;
                        }
                        if (flowChild.SelectNodes("amount").Count == 1 && flowChild.SelectNodes("capacity").Count == 1)
                        {
                            int amountInt = Int32.Parse(flowChild.SelectSingleNode("input/flow/stores/store/amount").InnerText); //directory not correct, todo
                            int capacitytInt = Int32.Parse(flowChild.SelectSingleNode("input/flow/stores/store/capacity").InnerText); //directory not correct, todo
                            if (capacitytInt > amountInt)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    if (flowChild.Name == "process")
                    {
                        if (flowChild.SelectNodes("id").Count > 1 || flowChild.SelectNodes("id").Count <= 0)
                        {
                            return true;
                        }
                        if (flowChild.SelectNodes("typIn").Count > 1 || flowChild.SelectNodes("typIn").Count <= 0)
                        {
                            return true;
                        }
                        if (flowChild.SelectNodes("typOut").Count > 1 || flowChild.SelectNodes("typOut").Count <= 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    if (flowChild.Name == "linkin" || flowChild.Name == "linkout")
                    {
                        if (flowChild.SelectNodes("id").Count > 1 || flowChild.SelectNodes("id").Count <= 0)
                        {
                            return true;
                        }
                        if (flowChild.SelectNodes("source").Count > 1 || flowChild.SelectNodes("source").Count <= 0)
                        {
                            return true;
                        }
                        if (flowChild.SelectNodes("target").Count > 1 || flowChild.SelectNodes("target").Count <= 0)
                        {
                            return true;
                        }
                        if (flowChild.SelectNodes("amount").Count > 1 || flowChild.SelectNodes("amount").Count <= 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                return false;
            }
            return false;
        }

        public static void Output(Object result)
        {
            SerializeToXMLFile(result, outFileName);
        }

        private static void SerializeToXMLFile(Object obj, String pathname)
        {
            //try
            //{
            XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
            using (TextWriter writer = File.CreateText(pathname))
            {
                xmlSerializer.Serialize(writer, obj);
            }
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("Failed to serialize: " + e.Message);
            //}
        }
    }
}