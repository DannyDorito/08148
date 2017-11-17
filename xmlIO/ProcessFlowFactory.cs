using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using logic; //uses the logic created in Logic.csproj
using System.Text.RegularExpressions;

namespace xmlIO
{
    /// <summary>
    /// All of the processing methods for the inputted XML document
    /// </summary>
    public class ProcessFlowFactory
    {
        /// <summary>
        /// Defined name of the output XML document
        /// </summary>
        // Defines the output XML name
        private const String outFileName = "out.xml";
        public static List<IFlowOperation> LoadInput(String pathname)
        {
            //Creates a new XML doc
            XmlDocument flowinXml = new XmlDocument();
            try
            {
                //Puts the read XML into the new XML doc
                using (TextReader reader = File.OpenText(pathname))
                {
                    flowinXml.Load(reader);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            //Puts the input child nodes into an XmlNodeList inputsXml
            XmlNodeList inputsXml = flowinXml.SelectNodes("input")[0].ChildNodes;
            List<IFlowOperation> result = new List<IFlowOperation>();

            foreach (XmlNode inputXml in inputsXml)
            {
                switch (inputXml.Name)
                {
                    //if the inputXml is flow
                    case "flow":
                        {
                            ProcessFlow processFlow = MkProcessFlow(inputXml);
                            //add the process to the results
                            result.Add(processFlow);
                            break;
                        }
                    //if the inputXml is execute
                    case "execute":
                        {
                            try
                            {
                                //add the execution to the results
                                result.Add(new OpExecute(ParseText(inputXml.InnerText)));
                            }
                            catch (Exception)
                            {
                                // do nothing
                            }
                            break;
                        }
                    //if the inputXml is query
                    case "query":
                        {
                            XmlNodeList storesXml = inputXml.SelectNodes("stores/store");
                            List<String> stores = new List<String>();
                            foreach (XmlNode storeXml in storesXml)
                            {
                                //add the store to stores
                                stores.Add(storeXml.InnerText);
                            }
                            //add the store to the results
                            result.Add(new OpQuery(stores));
                            break;
                        }
                    //if the inputXml is load
                    case "load":
                        {
                            XmlNodeList storeAmountsXml = inputXml.SelectNodes("storeAmount");
                            List<StoreIDAmount> storeAmounts = new List<StoreIDAmount>();

                            foreach (XmlNode storeAmountXml in storeAmountsXml)
                            {
                                XmlNodeList storeXml = storeAmountXml.SelectNodes("store");
                                XmlNodeList amountXml = storeAmountXml.SelectNodes("amount");
                                storeAmounts.Add(new StoreIDAmount(storeXml[0].InnerText, ParseText(amountXml[0].InnerText)));
                            }
                            //add the store to the results
                            result.Add(new OpLoad(storeAmounts));
                            break;
                        }
                    //if the inputXml is #comment
                    case "#comment":
                        {
                            break;
                        }
                    //else
                    default:
                        {
                            throw new Exception("Unknown XML: " + inputXml.Name);
                        }
                }
            }
            return result;
        }

        /// <summary>
        /// Loads the input XML  into an XML document
        /// </summary>
        /// <param name="pathname">the name of the input path</param>
        /// <returns></returns>
        public static List<int> LoadOutput(String pathname)
        {
            //create a new XML doc
            XmlDocument flowoutXml = new XmlDocument();
            try
            {
                //fill the XML doc with the results
                using (TextReader reader = File.OpenText(pathname))
                {
                    flowoutXml.Load(reader);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            XmlNodeList outputsXml = flowoutXml.SelectNodes("OpQueryOutput")[0].ChildNodes;
            List<int> result = new List<int>();
            foreach (XmlNode outputXml in outputsXml)
            {
                switch (outputXml.Name)
                {
                    //if outputXml is amounts
                    case "amounts":
                        {
                            XmlNodeList amountsXml = outputXml.SelectNodes("int");
                            foreach (XmlNode storeXml in amountsXml)
                            {
                                //add the store to the results
                                result.Add(ParseText(storeXml.InnerText));
                            }
                            break;
                        }
                    //else
                    default:
                        {
                            throw new Exception("Unknown XML: " + outputXml.Name);
                        }
                }
            }
            return result;
        }

        /// <summary>
        /// Try parse Method
        /// </summary>
        /// <param name="input">input string to parse into an int</param>
        /// <returns>int if the text parsed or 0</returns>
        public static int ParseText(string input)
        {
            int.TryParse(input, out int output);
            return output;
        }

        /// <summary>
        /// Processes the store portion of the inputted XML doc
        /// </summary>
        /// <param name="storeXml">The XML node storage for store</param>
        /// <returns>A list of store</returns>
        public static Store MkStore(XmlNode storeXml)
        {
            XmlNodeList idXml = storeXml.SelectNodes("id");
            String id = idXml[0].InnerText;
            XmlNodeList typXml = storeXml.SelectNodes("typ");
            String typ = typXml[0].InnerText;
            XmlNodeList amountXml = storeXml.SelectNodes("amount");

            int amount = ParseText(amountXml[0].InnerText);

            //optional
            XmlNodeList capacityXml = storeXml.SelectNodes("capacity");
            String capacityData = null;
            if (capacityXml.Count > 0)
            {
                capacityData = capacityXml[0].InnerText;

                int capacity = ParseText(capacityData);

                //bad workaround for capacity = 0, todo
                if (capacity <= amount && capacity != 0)
                {
                    return new Store(id, typ, amount, capacity);
                }
            }
            return new Store(id, typ, amount);
        }

        /// <summary>
        /// Processes the stores portion of the inputted XML doc
        /// </summary>
        /// <param name="storesXml">The XML node storage for stores</param>
        /// <returns>A list of stores</returns>
        public static Stores MkStores(XmlNode storesXml)
        {
            List<Store> stores = new List<Store>();
            XmlNodeList storeXml = storesXml.SelectNodes("store");
            foreach (XmlNode storXml in storeXml)
            {
                //call mkstore, add return value to stores
                stores.Add(MkStore(storXml));
            }
            return new Stores(stores);
        }

        /// <summary>
        /// Processes the process portion of the inputted XML doc
        /// </summary>
        /// <param name="processXml">The XML node storage for process</param>
        /// <returns>A list of process</returns>
        public static Process MkProcess(XmlNode processXml)
        {
            XmlNodeList idXml = processXml.SelectNodes("id");
            String id = idXml[0].InnerText;
            List<String> typsIn = new List<String>();
            XmlNodeList typsInXml = processXml.SelectNodes("typsIn");

            foreach (XmlNode typInXml in typsInXml)
            {
                //add typInXml to typsIn
                typsIn.Add(typInXml.FirstChild.InnerText);
            }

            List<String> typsOut = new List<String>();
            XmlNodeList typsOutXml = processXml.SelectNodes("typsOut");

            foreach (XmlNode typOutXml in typsOutXml)
            {
                //add typOutXml to typsOut
                typsOut.Add(typOutXml.FirstChild.InnerText);
            }
            return new Process(id, typsIn, typsOut);
        }

        /// <summary>
        /// Processes the processes portion of the inputted XML doc
        /// </summary>
        /// <param name="processesXml">The XML node storage for processes</param>
        /// <returns>A list of Processes</returns>
        public static Processes MkProcesses(XmlNode processesXml)
        {
            List<Process> processes = new List<Process>();
            XmlNodeList procsXml = processesXml.SelectNodes("process");

            foreach (XmlNode procXml in procsXml)
            {
                //call mkprocess, add return value to processes
                processes.Add(MkProcess(procXml));
            }
            return new Processes(processes);
        }

        /// <summary>
        /// Processes the links portion of the inputted XML doc
        /// </summary>
        /// <param name="linksXml">XML node to store each link object </param>
        /// <param name="stores">XML node stores</param>
        /// <param name="processes">XML bode processes</param>
        /// <returns>List of Links</returns>
        public static Links MkLinks(XmlNode linksXml, Stores stores, Processes processes)
        {
            List<Link> links = new List<Link>();
            XmlNodeList lnksXml = linksXml.SelectNodes("linkin");

            foreach (XmlNode lnkXml in lnksXml)
            {
                //call mklinkin to object l
                LinkIn l = MkLinkIn(lnkXml);
                //add l to list links
                links.Add(l);
            }

            lnksXml = linksXml.SelectNodes("linkout");

            foreach (XmlNode lnkXml in lnksXml)
            {
                //call mklinkout to object l
                LinkOut l = MkLinkOut(lnkXml);
                //add l to list links
                links.Add(l);
            }
            return new Links(links);
        }

        /// <summary>
        /// Creator of the LinkIn object
        /// </summary>
        /// <param name="node">the given link node</param>
        /// <returns>LinkIn object with properties id, sourceId, targetId, volume</returns>
        public static LinkIn MkLinkIn(XmlNode node)
        {
            String id = node.SelectNodes("id")[0].InnerText;
            String sourceId = node.SelectNodes("source")[0].InnerText;
            String targetId = node.SelectNodes("target")[0].InnerText;
            int volume = ParseText(node.SelectNodes("amount")[0].InnerText);

            return new LinkIn(id, sourceId, targetId, volume);
        }

        /// <summary>
        /// Creator of the LinkOut object
        /// </summary>
        /// <param name="node">the given link node</param>
        /// <returns>LinkOut object with properties id, sourceId, targetId, volume</returns>
        public static LinkOut MkLinkOut(XmlNode node)
        {
            String id = node.SelectNodes("id")[0].InnerText;
            String sourceId = node.SelectNodes("source")[0].InnerText;
            String targetId = node.SelectNodes("target")[0].InnerText;
            int volume = ParseText(node.SelectNodes("amount")[0].InnerText);

            return new LinkOut(id, sourceId, targetId, volume);
        }

        /// <summary>
        /// Creator of the Process Flow object
        /// </summary>
        /// <param name="flowXml">XML node that contains the flow child node in flowsXml</param>
        /// <returns>ProcessFlow object with properties stores, processes, links</returns>
        public static ProcessFlow MkProcessFlow(XmlNode flowXml)
        {
            XmlNodeList storesXml = flowXml.SelectNodes("stores");
            Stores stores = MkStores(storesXml[0]);
            XmlNodeList processesXml = flowXml.SelectNodes("processes");
            Processes processes = MkProcesses(processesXml[0]);
            XmlNodeList linksXml = flowXml.SelectNodes("links");
            Links links = MkLinks(linksXml[0], stores, processes);

            return new ProcessFlow(stores, processes, links);
        }

        /// <summary>
        /// Takes an input string and determines if the first char is a letter, and the others are numbers, input conformance
        /// </summary>
        /// <param name="input">string to evaluate if conforms</param>
        /// <returns>boolean if input is valid</returns>
        private bool StringInputIsValid(string input)
        {
            char[] inputChar = input.ToCharArray();

            if (inputChar.Length > 0)
            {
                for (int charNum = 0; charNum < inputChar.Length; charNum++)
                {
                    if (charNum == 0)
                    {
                        if (!Char.IsLetter(inputChar[0]))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!Char.IsNumber(inputChar[charNum]))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Method to output the results to an XML doc through serialization
        /// </summary>
        /// <param name="result">the resultant output of XML serialization</param>
        public static void Output(Object result)
        {
            SerializeToXMLFile(result, outFileName);
        }

        /// <summary>
        /// Method to serialize the output XML file and write output to target
        /// </summary>
        /// <param name="obj">the given object to serialize</param>
        /// <param name="pathname">pathname of the XML file</param>
        private static void SerializeToXMLFile(Object obj, String pathname)
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
                using (TextWriter writer = File.CreateText(pathname))
                {
                    xmlSerializer.Serialize(writer, obj);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to serialize: " + e.Message);
            }
        }
    }
}