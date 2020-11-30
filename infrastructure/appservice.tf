locals {
  resource_group_name  = "emmersion-learning-resources"
  location             = "westus"
  env                  = terraform.workspace == "production" ? "p" : "s"
  app_settings = {
    APPINSIGHTS_INSTRUMENTATIONKEY                  = "86f90d0a-9336-44e8-b1d5-f51a51623945"
    APPINSIGHTS_PROFILERFEATURE_VERSION             = "1.0.0"
    APPINSIGHTS_SNAPSHOTFEATURE_VERSION             = "1.0.0"
    APPLICATIONINSIGHTS_CONNECTION_STRING           = "InstrumentationKey=86f90d0a-9336-44e8-b1d5-f51a51623945"
  }
}

resource "azurerm_app_service" "distributed_tools_app_service" {
  name                = "e${local.env}-distributed-tools"
  app_service_plan_id = "emmersion-app-service-plan"
  resource_group_name = local.resource_group_name
  location            = local.location
  https_only          = true
  site_config {
    always_on         = true
    scm_type          = "VSTSRM"
    health_check_path = "/"
  }
  app_settings = local.app_settings
}
