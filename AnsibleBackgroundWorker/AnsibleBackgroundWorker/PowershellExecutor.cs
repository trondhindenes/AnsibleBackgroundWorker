using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace AnsibleBackgroundWorker
{

    public class PowershellExecutor
    {
        public static Runspace rs;
        public string VersionInfo = String.Empty;
        
        public PowershellExecutor()
        {
            rs = RunspaceFactory.CreateRunspace(InitialSessionState.CreateDefault());
            rs.Open();
            VersionInfo = rs.Version.ToString();

        }

        public List<string> Invoke(string script)
        {
            Pipeline pipeline = rs.CreatePipeline();
            pipeline.Commands.AddScript(script);
            var psResults = pipeline.Invoke();
            List<string> returnList = new List<string>();
            foreach (var psObject in psResults)
            {

                returnList.Add(psObject.ToString());
            }
/*            using (PowerShell ps = PowerShell.Create())
            {
                
                ps.Runspace = rs;
                ps.AddScript(script); 
                var psResults = ps.Invoke();
                Console.WriteLine($"Got {psResults.Count.ToString()} result objects back from Powershell");
                foreach (var psObject in psResults)
                {

                    returnList.Add(psObject.ToString());
                }
            }*/
            
            return returnList;
        }
    }
}