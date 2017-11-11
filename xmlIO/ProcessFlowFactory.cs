using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using logic; //uses the logic created in Logic.csproj

namespace xmlIO
{
    /// <summary>
    /// All of the prosessing methods for the inputted xml document
    /// </summary>
    public class ProcessFlowFactory
    {
        /// <summary>
        /// Defined name of the output xml document
        /// </summary>
        // Defines the output xml name
        private const String outFileName = "out.xml";
        public static List<IFlowOperation> LoadInput(String pathname)
        {
            //Creates a new xml doc
            XmlDocument flowinXml = new XmlDocument();
            //try //put this back on submission
            //{
            //Puts the read xml into the new xml doc
            using (TextReader reader = File.OpenText(pathname))
            {
                flowinXml.Load(reader);
            }
            //}
            //catch (Exception e) //put this back on submission
            //{
            //    Console.WriteLine(e.Message);
            //}
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
                            //try //put this back on submission
                            //{
                            //add the execution to the results
                            result.Add(new OpExecute(ParseText(inputXml.InnerText)));
                            //}
                            //catch (Exception e) //put this back on submission
                            //{
                            //    // do nothing
                            //}
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
                            throw new Exception("Unknown Xml: " + inputXml.Name);
                        }
                }
            }
            return result;
        }

        /// <summary>
        /// Loads the input xml  into an xml document
        /// </summary>
        /// <param name="pathname">the name of the input path</param>
        /// <returns></returns>
        public static List<int> LoadOutput(String pathname)
        {
            //create a new xml doc
            XmlDocument flowoutXml = new XmlDocument();
            //try //put this back on submission
            //{
            //fill the xml doc with the results
            using (TextReader reader = File.OpenText(pathname))
            {
                flowoutXml.Load(reader);
            }
            //}
            //catch (Exception e) //put this back on submission
            //{
            //    Console.WriteLine(e.Message);
            //}
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
                            throw new Exception("Unknown Xml: " + outputXml.Name); //this should be better, todo
                        }
                }
            }
            return result;
        }

        /// <summary>
        /// Tryparse Method
        /// </summary>
        /// <param name="input">input string to parse into an int</param>
        /// <returns>int if the text parsed or 0</returns>
        public static int ParseText(string input)
        {
            int.TryParse(input, out int output);
            return output;
        }

        /// <summary>
        /// Processes the store portion of the inputted xml doc
        /// </summary>
        /// <param name="storeXml">The xml node storage for store</param>
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

                return new Store(id, typ, amount, ParseText(capacityData));
            }
            //else
            return new Store(id, typ, amount);
        }

        /// <summary>
        /// Processes the stores portion of the inputted xml doc
        /// </summary>
        /// <param name="storesXml">The xml node storage for stores</param>
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
        /// Processes the process portion of the inputted xml doc
        /// </summary>
        /// <param name="processXml">The xml node storage for process</param>
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
        /// Processes the processes portion of the inputted xml doc
        /// </summary>
        /// <param name="processesXml">The xml node storage for processes</param>
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
        /// Processes the links portion of the inputted xml doc
        /// </summary>
        /// <param name="linksXml">xml node to store each link object </param>
        /// <param name="stores">xml node stores</param>
        /// <param name="processes">xml bode processes</param>
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
        /// <param name="node"></param> /todo
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
        /// <param name="node"></param> //todo
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
        /// <param name="flowXml">xml node that contains the flow child node in flowsXml</param>
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
        /// Method to calculate the specific process' process cost
        /// </summary>
        /// <param name="processes">todo</param>
        /// <param name="processId">todo</param>
        /// <returns>todo</returns>
        public static int ProcessCost(Processes processes, string processId)
        {
            int processCost = 0;
            //processCost = process processId (input resource cost + output resource cost), todo
            return processCost;
        }

        /// <summary>
        /// Method to output the results to an xml doc through serialization
        /// </summary>
        /// <param name="result"></param> //todo
        public static void Output(Object result)
        {
            SerializeToXMLFile(result, outFileName);
        }

        /// <summary>
        /// Method to serialize the output Xml file and write output to target
        /// </summary>
        /// <param name="obj">todo</param>
        /// <param name="pathname">pathname of the Xml file</param>
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