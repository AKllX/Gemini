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
#include "MasterTestFixture.h"

#include <asiodnp3/DefaultMasterApplication.h>

#include <testlib/BufferHelpers.h>

using namespace testlib;

namespace opendnp3
{

MasterParams NoStartupTasks()
{
    MasterParams params;
    params.disableUnsolOnStartup = false;
    params.startupIntegrityClassMask = ClassField::None();
    params.unsolClassMask = ClassField::None();
    return params;
}

MasterTestFixture::MasterTestFixture(const MasterParams& params,
                                     const Addresses& addresses,
                                     const std::string& id,
                                     const std::shared_ptr<openpal::ILogHandler>& log,
                                     const std::shared_ptr<testlib::MockExecutor>& executor,
                                     const std::shared_ptr<IMasterScheduler>& scheduler)
    : addresses(addresses),
      log(log),
      exe(executor ? executor : std::make_shared<MockExecutor>()),
      meas(std::make_shared<MockSOEHandler>()),
      lower(std::make_shared<MockLowerLayer>()),
      application(std::make_shared<MockMasterApplication>()),
      scheduler(scheduler ? scheduler : std::make_shared<MasterSchedulerBackend>(exe)),
      context(std::make_shared<MContext>(
          addresses, openpal::Logger(log, id, ~0), exe, lower, meas, application, this->scheduler, params))
{
}

MasterTestFixture::~MasterTestFixture()
{
    this->scheduler->Shutdown();
}

bool MasterTestFixture::SendToMaster(const std::string& hex)
{
    HexSequence hs(hex);
    return context->OnReceive(Message(this->addresses.Reverse(), hs.ToRSlice()));
}

} // namespace opendnp3
