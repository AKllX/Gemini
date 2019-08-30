/*
 * Copyright 2013-2019 Automatak, LLC
 *
 * Licensed to Green Energy Corp (www.greenenergycorp.com) and Automatak
 * LLC (www.automatak.com) under one or more contributor license agreements.
 * See the NOTICE file distributed with this work for additional information
 * regarding copyright ownership. Green Energy Corp and Automatak LLC license
 * this file to you under the Apache License, Version 2.0 (the "License"); you
 * may not use this file except in compliance with the License. You may obtain
 * a copy of the License at:
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
#include "asiodnp3/MasterScan.h"

#include "openpal/executor/IExecutor.h"

#include "opendnp3/master/IMasterTask.h"
#include "opendnp3/master/MasterContext.h"

#include <utility>

using namespace openpal;
using namespace opendnp3;

namespace asiodnp3
{

MasterScan::MasterScan(std::shared_ptr<opendnp3::IMasterTask> task, std::shared_ptr<IMasterScheduler> scheduler)
    : task(std::move(task)), scheduler(std::move(scheduler))
{
}

void MasterScan::Demand()
{
    scheduler->Demand(task);
}

} // namespace asiodnp3
