# AnsibleBackgroundWorker

WARNING: RESEARCH STAGE. DO NOT RUN IN PRODUCTION

### What is dis:
AnsibleBackgroundWorker doesnt really directly related to Ansible - its a tcp listener which can receive powershell scripts (on telnet) and executes them.
It implements a "global" PowerShell runspace in order to make things as speedy as possible. Very much work in progress!
