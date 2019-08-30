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
package com.automatak.dnp3;

public final class LogMasks
{
    public static final int NONE = 0;
    public static final int ALL = ~NONE;
    public static final int NORMAL = LogLevels.EVENT | LogLevels.ERROR | LogLevels.WARNING | LogLevels.INFO;
    public static final int APP_COMMS = LogLevels.APP_HEADER_RX | LogLevels.APP_HEADER_TX | LogLevels.APP_OBJECT_RX | LogLevels.APP_OBJECT_TX;
}
