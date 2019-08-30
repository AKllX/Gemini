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
#ifndef OPENPAL_FORMAT_H
#define OPENPAL_FORMAT_H

#include "openpal/container/WSlice.h"
#include "openpal/serialization/UInt48Type.h"
#include "openpal/util/Uncopyable.h"

namespace openpal
{
class Format : private StaticOnly
{
public:
    static bool Write(WSlice& dest, const uint8_t& value);
    static bool Write(WSlice& dest, const uint16_t& value);

    static bool Write(WSlice& dest, const uint32_t& value);
    static bool Write(WSlice& dest, const UInt48Type& value);

    static bool Write(WSlice& dest, const int16_t& value);
    static bool Write(WSlice& dest, const int32_t& value);

    static bool Write(WSlice& dest, const double& value);
    static bool Write(WSlice& dest, const float& value);

    template<typename T, typename... Args> static bool Many(WSlice& dest, const T& value, const Args&... args)
    {
        return Write(dest, value) && Many(dest, args...);
    }

private:
    static bool Many(WSlice& input)
    {
        return true;
    }
};

} // namespace openpal

#endif
