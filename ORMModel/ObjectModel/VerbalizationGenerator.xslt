<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:plx="http://Schemas.Neumont.edu/CodeGeneration/Plix"
    xmlns:ve="http://Schemas.Neumont.edu/ORM/SDK/Verbalization"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt"
	>
	<xsl:preserve-space elements="ve:Form"/>
	<!-- Pick up param value supplied automatically by plix loader -->
	<xsl:param name="CustomToolNamespace" select="'TestNamespace'"/>

	<!-- Names of the different classes we generate -->
	<xsl:param name="VerbalizationTextSnippetType" select="'VerbalizationTextSnippetType'"/>
	<xsl:param name="VerbalizationSet" select="'VerbalizationSet'"/>
	<xsl:param name="VerbalizationSets" select="'VerbalizationSets'"/>

	<!-- Include templates to generate the shared verbalization classes -->
	<xsl:include href="VerbalizationGenerator.Sets.xslt"/>
	<xsl:template match="ve:Root">
		<plx:Root>
			<plx:Using name="System"/>
			<plx:Using name="System.Text"/>
			<plx:Using name="System.Globalization"/>
			<plx:Namespace name="{$CustomToolNamespace}">
				<!-- Generate verbalization set classes and default populations -->
				<xsl:call-template name="GenerateVerbalizationSets"/>
				<!-- Generate verbalization implementations for constraints -->
				<xsl:call-template name="GenerateConstraintVerbalization"/>
			</plx:Namespace>
		</plx:Root>
	</xsl:template>
	<xsl:template name="GenerateConstraintVerbalization">
		<xsl:apply-templates select="ve:Constructs/ve:Constraints/ve:Constraint" mode="ConstraintVerbalization"/>
	</xsl:template>
	<xsl:template match="ve:Constraint[@patternGroup='InternalConstraint']" mode="ConstraintVerbalization">
		<plx:Class name="{@type}" visibility="Public" partial="true">
			<plx:ImplementsInterface dataTypeName="IVerbalize"/>
			<plx:Function name="GetVerbalization" visibility="Protected">
				<plx:Param name="" type="RetVal" dataTypeName="String" dataTypeQualifier="System"/>
				<plx:Param name="isNegative" dataTypeName="Boolean" dataTypeQualifier="System"/>
				<plx:InterfaceMember member="GetVerbalization" dataTypeName="IVerbalize"/>
				<plx:Variable name="sbMain" dataTypeName="StringBuilder">
					<plx:Initialize>
						<plx:NullObjectKeyword/>
					</plx:Initialize>
				</plx:Variable>
				<plx:Variable name="sbTemp" dataTypeName="StringBuilder">
					<plx:Initialize>
						<plx:NullObjectKeyword/>
					</plx:Initialize>
				</plx:Variable>

				<!-- Don't proceed with verbalization if errors are present -->
				<plx:Variable name="errorOwner" dataTypeName="IModelErrorOwner">
					<plx:Initialize>
						<plx:Cast type="TypeCastTest">
							<plx:TargetType dataTypeName="IModelErrorOwner"/>
							<plx:CastExpression>
								<plx:ThisKeyword/>
							</plx:CastExpression>
						</plx:Cast>
					</plx:Initialize>
				</plx:Variable>
				<plx:Condition>
					<plx:Test>
						<plx:Operator type="IdentityInequality">
							<plx:Left>
								<plx:Value type="Local" data="errorOwner"/>
							</plx:Left>
							<plx:Right>
								<plx:NullObjectKeyword/>
							</plx:Right>
						</plx:Operator>
					</plx:Test>
					<plx:Body>
						<plx:Iterator variableName="error" dataTypeName="ModelError">
							<plx:Initialize>
								<plx:CallInstance name="ErrorCollection" type="Property">
									<plx:CallObject>
										<plx:Value type="Local" data="errorOwner"/>
									</plx:CallObject>
								</plx:CallInstance>
							</plx:Initialize>
							<plx:Body>
								<plx:Condition>
									<plx:Test>
										<plx:Operator type="IdentityInequality">
											<plx:Left>
												<plx:Value type="Local" data="sbMain"/>
											</plx:Left>
											<plx:Right>
												<plx:NullObjectKeyword/>
											</plx:Right>
										</plx:Operator>
									</plx:Test>
									<plx:Body>
										<plx:Operator type="Assign">
											<plx:Left>
												<plx:Value type="Local" data="sbMain"/>
											</plx:Left>
											<plx:Right>
												<plx:CallNew dataTypeName="StringBuilder"/>
											</plx:Right>
										</plx:Operator>
									</plx:Body>
									<plx:Alternate>
										<plx:CallInstance name="AppendLine">
											<plx:CallObject>
												<plx:Value type="Local" data="sbMain"/>
											</plx:CallObject>
										</plx:CallInstance>
									</plx:Alternate>
								</plx:Condition>
								<plx:CallInstance name="Append">
									<plx:CallObject>
										<plx:Value type="Local" data="sbMain"/>
									</plx:CallObject>
									<plx:PassParam>
										<plx:CallInstance name="Name" type="Property">
											<plx:CallObject>
												<plx:Value type="Local" data="error"/>
											</plx:CallObject>
										</plx:CallInstance>
									</plx:PassParam>
								</plx:CallInstance>
							</plx:Body>
						</plx:Iterator>
						<plx:Condition>
							<plx:Test>
								<plx:Operator type="IdentityInequality">
									<plx:Left>
										<plx:Value type="Local" data="sbMain"/>
									</plx:Left>
									<plx:Right>
										<plx:NullObjectKeyword/>
									</plx:Right>
								</plx:Operator>
							</plx:Test>
							<plx:Body>
								<plx:Return>
									<plx:CallInstance name="ToString">
										<plx:CallObject>
											<plx:Value type="Local" data="sbMain"/>
										</plx:CallObject>
									</plx:CallInstance>
								</plx:Return>
							</plx:Body>
						</plx:Condition>
					</plx:Body>
				</plx:Condition>

				<!-- Pick up standard code we'll need for any constraint -->
				<plx:Variable name="snippets" dataTypeName="{$VerbalizationSets}">
					<plx:Initialize>
						<plx:CallStatic dataTypeName="{$VerbalizationSets}" name="Default" type="Property"/>
					</plx:Initialize>
				</plx:Variable>
				<plx:Variable name="isDeontic" dataTypeName="Boolean" dataTypeQualifier="System">
					<plx:Initialize>
						<plx:FalseKeyword/>
					</plx:Initialize>
				</plx:Variable>

				<!-- Pick up standard code we'll need for any internal constraint -->
				<!-- UNDONE: We'll need to switch off a patternGroup specified here -->
				<plx:Variable name="parentFact" dataTypeName="FactType">
					<plx:Initialize>
						<plx:CallInstance name="FactType" type="Property">
							<plx:CallObject>
								<plx:ThisKeyword/>
							</plx:CallObject>
						</plx:CallInstance>
					</plx:Initialize>
				</plx:Variable>
				<plx:Variable name="includedRoles" dataTypeName="RoleMoveableCollection">
					<plx:Initialize>
						<plx:CallInstance name="RoleCollection" type="Property">
							<plx:CallObject>
								<plx:ThisKeyword/>
							</plx:CallObject>
						</plx:CallInstance>
					</plx:Initialize>
				</plx:Variable>
				<plx:Variable name="allRoles" dataTypeName="RoleMoveableCollection">
					<plx:Initialize>
						<plx:CallInstance name="RoleCollection" type="Property">
							<plx:CallObject>
								<plx:Value type="Local" data="parentFact"/>
							</plx:CallObject>
						</plx:CallInstance>
					</plx:Initialize>
				</plx:Variable>
				<plx:Variable name="allReadingOrders" dataTypeName="ReadingOrderMoveableCollection">
					<plx:Initialize>
						<plx:CallInstance name="ReadingOrderCollection" type="Property">
							<plx:CallObject>
								<plx:Value type="Local" data="parentFact"/>
							</plx:CallObject>
						</plx:CallInstance>
					</plx:Initialize>
				</plx:Variable>

				<!-- No readings is an error on the parent, so we can get past the error check without them -->
				<plx:Condition>
					<plx:Test>
						<plx:Operator type="Equality">
							<plx:Left>
								<plx:CallInstance name="Count" type="Property">
									<plx:CallObject>
										<plx:Value type="Local" data="allReadingOrders"/>
									</plx:CallObject>
								</plx:CallInstance>
							</plx:Left>
							<plx:Right>
								<plx:Value type="I4" data="0"/>
							</plx:Right>
						</plx:Operator>
					</plx:Test>
					<plx:Body>
						<plx:Return>
							<plx:String/>
						</plx:Return>
					</plx:Body>
				</plx:Condition>
				<plx:Variable name="includedArity" dataTypeName="Int32" dataTypeQualifier="System">
					<plx:Initialize>
						<plx:CallInstance name="Count" type="Property">
							<plx:CallObject>
								<plx:Value type="Local" data="includedRoles"/>
							</plx:CallObject>
						</plx:CallInstance>
					</plx:Initialize>
				</plx:Variable>
				<plx:Variable name="fullArity" dataTypeName="Int32" dataTypeQualifier="System">
					<plx:Initialize>
						<plx:CallInstance name="Count" type="Property">
							<plx:CallObject>
								<plx:Value type="Local" data="allRoles"/>
							</plx:CallObject>
						</plx:CallInstance>
					</plx:Initialize>
				</plx:Variable>
				<plx:Variable name="basicRoleReplacements" dataTypeName="String" dataTypeQualifier="System" dataTypeIsSimpleArray="true">
					<plx:Initialize>
						<plx:CallNew dataTypeName="String" dataTypeQualifier="System" dataTypeIsSimpleArray="true">
							<plx:PassParam>
								<plx:Value type="Local" data="fullArity"/>
							</plx:PassParam>
						</plx:CallNew>
					</plx:Initialize>
				</plx:Variable>
				<plx:Variable name="roleReplacements" dataTypeName="String" dataTypeQualifier="System" dataTypeIsSimpleArray="true">
					<plx:Initialize>
						<plx:CallNew dataTypeName="String" dataTypeQualifier="System" dataTypeIsSimpleArray="true">
							<plx:PassParam>
								<plx:Value type="Local" data="fullArity"/>
							</plx:PassParam>
						</plx:CallNew>
					</plx:Initialize>
				</plx:Variable>
				<plx:Operator type="Assign">
					<plx:Left>
						<plx:Value type="Local" data="sbMain"/>
					</plx:Left>
					<plx:Right>
						<plx:CallNew dataTypeName="StringBuilder"/>
					</plx:Right>
				</plx:Operator>
				<plx:Variable name="readingOrder" dataTypeName="ReadingOrder"/>
				<plx:Variable name="i" dataTypeName="Int32" dataTypeQualifier="System">
					<plx:Initialize>
						<plx:Value type="I4" data="0"/>
					</plx:Initialize>
				</plx:Variable>
				<plx:Loop>
					<plx:LoopTest>
						<plx:Operator type="LessThan">
							<plx:Left>
								<plx:Value type="Local" data="i"/>
							</plx:Left>
							<plx:Right>
								<plx:Value type="Local" data="fullArity"/>
							</plx:Right>
						</plx:Operator>
					</plx:LoopTest>
					<plx:LoopIncrement>
						<plx:Operator type="Assign">
							<plx:Left>
								<plx:Value type="Local" data="i"/>
							</plx:Left>
							<plx:Right>
								<plx:Operator type="Add">
									<plx:Left>
										<plx:Value type="Local" data="i"/>
									</plx:Left>
									<plx:Right>
										<plx:Value type="I4" data="1"/>
									</plx:Right>
								</plx:Operator>
							</plx:Right>
						</plx:Operator>
					</plx:LoopIncrement>
					<plx:Body>
						<plx:Variable name="rolePlayer" dataTypeName="ObjectType">
							<plx:Initialize>
								<plx:CallInstance name="RolePlayer" type="Property">
									<plx:CallObject>
										<plx:CallInstance name="" type="Indexer">
											<plx:CallObject>
												<plx:Value type="Local" data="allRoles"/>
											</plx:CallObject>
											<plx:PassParam>
												<plx:Value type="Local" data="i"/>
											</plx:PassParam>
										</plx:CallInstance>
									</plx:CallObject>
								</plx:CallInstance>
							</plx:Initialize>
						</plx:Variable>
						<plx:Variable name="basicReplacement" dataTypeName="String" dataTypeQualifier="System"/>
						<!--UNDONE: Ring situations-->
						<!--UNDONE: Localize or pull the role name from the snippet set-->
						<plx:Condition>
							<plx:Test>
								<plx:Operator type="IdentityInequality">
									<plx:Left>
										<plx:Value type="Local" data="rolePlayer"/>
									</plx:Left>
									<plx:Right>
										<plx:NullObjectKeyword/>
									</plx:Right>
								</plx:Operator>
							</plx:Test>
							<plx:Body>
								<plx:Operator type="Assign">
									<plx:Left>
										<plx:Value type="Local" data="basicReplacement"/>
									</plx:Left>
									<plx:Right>
										<plx:CallInstance name="Name" type="Property">
											<plx:CallObject>
												<plx:Value type="Local" data="rolePlayer"/>
											</plx:CallObject>
										</plx:CallInstance>
									</plx:Right>
								</plx:Operator>
							</plx:Body>
							<plx:Alternate>
								<plx:Operator type="Assign">
									<plx:Left>
										<plx:Value type="Local" data="basicReplacement"/>
									</plx:Left>
									<plx:Right>
										<plx:Operator type="Add">
											<plx:Left>
												<plx:String>Role</plx:String>
											</plx:Left>
											<plx:Right>
												<plx:CallInstance name="ToString">
													<plx:CallObject>
														<plx:Expression parens="true">
															<plx:Operator type="Add">
																<plx:Left>
																	<plx:Value type="Local" data="i"/>
																</plx:Left>
																<plx:Right>
																	<plx:Value type="I4" data="1"/>
																</plx:Right>
															</plx:Operator>
														</plx:Expression>
													</plx:CallObject>
													<plx:PassParam>
														<plx:CallStatic name="CurrentUICulture" dataTypeName="CultureInfo" type="Property"/>
													</plx:PassParam>
												</plx:CallInstance>
											</plx:Right>
										</plx:Operator>
									</plx:Right>
								</plx:Operator>
							</plx:Alternate>
						</plx:Condition>
						<plx:Operator type="Assign">
							<plx:Left>
								<plx:CallInstance name="" type="ArrayIndexer">
									<plx:CallObject>
										<plx:Value type="Local" data="basicRoleReplacements"/>
									</plx:CallObject>
									<plx:PassParam>
										<plx:Value type="Local" data="i"/>
									</plx:PassParam>
								</plx:CallInstance>
							</plx:Left>
							<plx:Right>
								<plx:Value type="Local" data="basicReplacement"/>
							</plx:Right>
						</plx:Operator>
					</plx:Body>
				</plx:Loop>
				<!-- Copy the constraint roles into an ordered set so that the following-sibling
					 axis can be used to walk the ordered elements. The data is needed because a
					 sorted for-each set does not affect the following-sibling axis order, which
					 is relied on by the InternalConstraintArityConditions && InternalConstraintSpanConditions
					 helper templates. -->
				<xsl:variable name="sortedConstrainedRoles">
					<xsl:for-each select="ve:ConstrainedRoles">
						<!-- The actual order does not matter for specified fields,
					     but we want unspecified fields last, so sort descending -->
						<xsl:sort select="@arity" data-type="text" order="descending"/>
						<xsl:sort select="@span" data-type="text" order="descending"/>
						<xsl:copy-of select="."/>
					</xsl:for-each>
				</xsl:variable>
				<xsl:for-each select="msxsl:node-set($sortedConstrainedRoles)/ve:ConstrainedRoles">
					<xsl:if test="position()=1">
						<xsl:choose>
							<!-- An arity match is specified, use as the outer check -->
							<xsl:when test="string-length(@arity)">
								<xsl:call-template name="InternalConstraintArityConditions"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:call-template name="InternalConstraintSpanConditions"/>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:if>
				</xsl:for-each>
				<plx:Return>
					<plx:CallInstance name="ToString">
						<plx:CallObject>
							<plx:Value type="Local" data="sbMain"/>
						</plx:CallObject>
					</plx:CallInstance>
				</plx:Return>
			</plx:Function>
		</plx:Class>
	</xsl:template>
	<!-- Helper template for spitting conditional pattern matches based on arity. The assumption
		 is made that the arity attribute for the current node is set, and that the elements in
		 the parent context are ordered by arity/span (non-blank elements first). This defers
		 to the InternalConstraintSpanConditions template for all body content. -->
	<xsl:template name="InternalConstraintArityConditions">
		<xsl:param name="fallback" select="false()"/>
		<xsl:variable name="matchArity" select="@arity"/>
		<xsl:choose>
			<xsl:when test="$fallback">
				<xsl:choose>
					<xsl:when test="string-length(@arity)">
						<plx:FallbackCondition>
							<plx:Test>
								<plx:Operator type="Equality">
									<plx:Left>
										<plx:Value type="Local" data="fullArity"/>
									</plx:Left>
									<plx:Right>
										<plx:Value type="I4" data="{$matchArity}"/>
									</plx:Right>
								</plx:Operator>
							</plx:Test>
							<plx:Body>
								<xsl:call-template name="InternalConstraintSpanConditions">
									<xsl:with-param name="contextArity" select="$matchArity"/>
								</xsl:call-template>
							</plx:Body>
						</plx:FallbackCondition>
						<xsl:for-each select="following-sibling::*[not(@arity=$matchArity)]">
							<xsl:if test="position()=1">
								<xsl:call-template name="InternalConstraintArityConditions">
									<xsl:with-param name="fallback" select="true()"/>
									<xsl:with-param name="contextArity" select="@arity"/>
								</xsl:call-template>
							</xsl:if>
						</xsl:for-each>
					</xsl:when>
					<xsl:otherwise>
						<plx:Alternate>
							<xsl:call-template name="InternalConstraintSpanConditions"/>
						</plx:Alternate>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<xsl:otherwise>
				<plx:Condition>
					<plx:Test>
						<plx:Operator type="Equality">
							<plx:Left>
								<plx:Value type="Local" data="fullArity"/>
							</plx:Left>
							<plx:Right>
								<plx:Value type="I4" data="{$matchArity}"/>
							</plx:Right>
						</plx:Operator>
					</plx:Test>
					<plx:Body>
						<xsl:call-template name="InternalConstraintSpanConditions">
							<xsl:with-param name="contextArity" select="$matchArity"/>
						</xsl:call-template>
					</plx:Body>
					<xsl:for-each select="following-sibling::*[not(@arity=$matchArity)]">
						<xsl:if test="position()=1">
							<xsl:call-template name="InternalConstraintArityConditions">
								<xsl:with-param name="fallback" select="true()"/>
							</xsl:call-template>
						</xsl:if>
					</xsl:for-each>
				</plx:Condition>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- Helper template for spitting conditional pattern matches based on span. -->
	<xsl:template name="InternalConstraintSpanConditions">
		<xsl:param name="fallback" select="false()"/>
		<xsl:param name="contextArity" select="''"/>
		<xsl:variable name="matchSpan" select="@span"/>
		<xsl:choose>
			<xsl:when test="$fallback">
				<xsl:choose>
					<xsl:when test="$matchSpan='All'">
						<plx:FallbackCondition>
							<plx:Test>
								<plx:Operator type="Equality">
									<plx:Left>
										<plx:Value type="Local" data="fullArity"/>
									</plx:Left>
									<plx:Right>
										<plx:Value type="Local" data="includedArity"/>
									</plx:Right>
								</plx:Operator>
							</plx:Test>
							<plx:Body>
								<xsl:call-template name="InternalConstraintBodyContent"/>
							</plx:Body>
						</plx:FallbackCondition>
						<xsl:for-each select="following-sibling::*[not(@span=$matchSpan) and string(@arity)=$contextArity]">
							<xsl:if test="position()=1">
								<xsl:call-template name="InternalConstraintSpanConditions">
									<xsl:with-param name="fallback" select="true()"/>
									<xsl:with-param name="contextArity" select="@contextArity"/>
								</xsl:call-template>
							</xsl:if>
						</xsl:for-each>
					</xsl:when>
					<xsl:otherwise>
						<plx:Alternate>
							<xsl:call-template name="InternalConstraintBodyContent"/>
						</plx:Alternate>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<xsl:otherwise>
				<xsl:choose>
					<xsl:when test="$matchSpan='All'">
						<plx:Condition>
							<plx:Test>
								<plx:Operator type="Equality">
									<plx:Left>
										<plx:Value type="Local" data="fullArity"/>
									</plx:Left>
									<plx:Right>
										<plx:Value type="Local" data="includedArity"/>
									</plx:Right>
								</plx:Operator>
							</plx:Test>
							<plx:Body>
								<xsl:call-template name="InternalConstraintBodyContent"/>
							</plx:Body>
							<xsl:for-each select="following-sibling::*[not(@span=$matchSpan) and string(@arity)=$contextArity]">
								<xsl:if test="position()=1">
									<xsl:call-template name="InternalConstraintSpanConditions">
										<xsl:with-param name="fallback" select="true()"/>
										<xsl:with-param name="contextArity" select="@contextArity"/>
									</xsl:call-template>
								</xsl:if>
							</xsl:for-each>
						</plx:Condition>
					</xsl:when>
					<xsl:otherwise>
						<xsl:call-template name="InternalConstraintBodyContent"/>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template name="InternalConstraintBodyContent">
		<!-- At this point we'll either have ConditionalReading or Quantifier children -->
		<xsl:apply-templates select="child::*" mode="InternalConstraintVerbalization">
			<xsl:with-param name="TopLevel" select="true()"/>
		</xsl:apply-templates>
	</xsl:template>
	<xsl:template match="ve:ConditionalReading" mode="InternalConstraintVerbalization">
		<xsl:param name="TopLevel" select="false()"/>
		<xsl:param name="IteratorContext" select="'all'"/>
		<xsl:if test="$TopLevel and position()&gt;1">
			<plx:CallInstance name="AppendLine">
				<plx:CallObject>
					<plx:Value type="Local" data="sbMain"/>
				</plx:CallObject>
			</plx:CallInstance>
		</xsl:if>
		<xsl:for-each select="ve:ReadingChoice">
			<xsl:if test="position()=1">
				<xsl:call-template name="ProcessConditionalReadingChoice">
					<xsl:with-param name="IteratorContext" select="$IteratorContext"/>
				</xsl:call-template>
			</xsl:if>
		</xsl:for-each>
	</xsl:template>
	<xsl:template name="ProcessConditionalReadingChoice">
		<xsl:param name="IteratorContext" select="'all'"/>
		<xsl:call-template name="PopulateReadingOrder">
			<xsl:with-param name="readingChoice" select="@match"/>
		</xsl:call-template>
		<plx:Condition>
			<plx:Test>
				<plx:Operator type="IdentityInequality">
					<plx:Left>
						<plx:Value type="Local" data="readingOrder"/>
					</plx:Left>
					<plx:Right>
						<plx:NullObjectKeyword/>
					</plx:Right>
				</plx:Operator>
			</plx:Test>
			<plx:Body>
				<xsl:apply-templates select="child::*" mode="InternalConstraintVerbalization">
					<xsl:with-param name="IteratorContext" select="$IteratorContext"/>
				</xsl:apply-templates>
			</plx:Body>
			<xsl:if test="position()!=last()">
				<plx:Alternate>
					<xsl:for-each select="following-sibling::*">
						<xsl:if test="position()=1">
							<xsl:call-template name="ProcessConditionalReadingChoice">
								<xsl:with-param name="IteratorContext" select="$IteratorContext"/>
							</xsl:call-template>
						</xsl:if>
					</xsl:for-each>
				</plx:Alternate>
			</xsl:if>
		</plx:Condition>
	</xsl:template>
	<xsl:template match="ve:Quantifier" mode="InternalConstraintVerbalization">
		<xsl:param name="VariableDecorator" select="position()"/>
		<xsl:param name="VariablePrefix" select="'snippet'"/>
		<xsl:param name="TopLevel" select="false()"/>
		<xsl:param name="IteratorContext" select="'all'"/>
		<xsl:if test="$TopLevel and position()&gt;1">
			<plx:CallInstance name="AppendLine">
				<plx:CallObject>
					<plx:Value type="Local" data="sbMain"/>
				</plx:CallObject>
			</plx:CallInstance>
		</xsl:if>
		<plx:Variable name="{$VariablePrefix}{$VariableDecorator}" dataTypeName="String" dataTypeQualifier="System">
			<plx:Initialize>
				<xsl:call-template name="SnippetFor">
					<xsl:with-param name="SnippetType" select="@type"/>
				</xsl:call-template>
			</plx:Initialize>
		</plx:Variable>
		<xsl:for-each select="child::*">
			<plx:Variable name="{$VariablePrefix}{$VariableDecorator}replace{position()}" dataTypeName="String" dataTypeQualifier="System">
				<plx:Initialize>
					<plx:NullObjectKeyword/>
				</plx:Initialize>
			</plx:Variable>
			<xsl:apply-templates select="."  mode="InternalConstraintVerbalization">
				<xsl:with-param name="VariablePrefix" select="concat($VariablePrefix,$VariableDecorator,'replace')"/>
				<xsl:with-param name="IteratorContext" select="$IteratorContext"/>
			</xsl:apply-templates>
		</xsl:for-each>
		<plx:CallInstance name="AppendFormat">
			<plx:CallObject>
				<plx:Value type="Local" data="sbMain"/>
			</plx:CallObject>
			<plx:PassParam>
				<plx:CallStatic dataTypeName="CultureInfo" name="CurrentUICulture" type="Property"/>
			</plx:PassParam>
			<plx:PassParam>
				<plx:Value type="Local" data="{$VariablePrefix}{$VariableDecorator}"/>
			</plx:PassParam>
			<xsl:for-each select="child::*">
				<plx:PassParam>
					<plx:Value type="Local" data="{$VariablePrefix}{$VariableDecorator}replace{position()}"/>
				</plx:PassParam>
			</xsl:for-each>
		</plx:CallInstance>
	</xsl:template>
	<xsl:template match="ve:Fact" mode="InternalConstraintVerbalization">
		<xsl:param name="VariableDecorator" select="position()"/>
		<xsl:param name="VariablePrefix" select="'factText'"/>
		<!-- all, included or excluded are the supported IteratorContext -->
		<xsl:param name="IteratorContext" select="'all'"/>
		<xsl:variable name="complexReplacement" select="0!=count(ve:PredicateReplacement)"/>
		<xsl:call-template name="PopulateReadingOrder">
			<xsl:with-param name="readingChoice" select="@readingChoice"/>
		</xsl:call-template>
		<xsl:choose>
			<xsl:when test="$complexReplacement">
				<xsl:variable name="iterVarName" select="concat($VariablePrefix,'factRoleIter',$VariableDecorator)"/>
				<plx:Variable name="{$iterVarName}" dataTypeName="Int32" dataTypeQualifier="System">
					<plx:Initialize>
						<plx:Value type="I4" data="0"/>
					</plx:Initialize>
				</plx:Variable>
				<plx:Loop>
					<plx:LoopTest apply="Before">
						<plx:Operator type="LessThan">
							<plx:Left>
								<plx:Value type="Local" data="{$iterVarName}"/>
							</plx:Left>
							<plx:Right>
								<plx:Value type="Local" data="fullArity"/>
							</plx:Right>
						</plx:Operator>
					</plx:LoopTest>
					<plx:LoopIncrement>
						<plx:Operator type="Assign">
							<plx:Left>
								<plx:Value type="Local" data="{$iterVarName}"/>
							</plx:Left>
							<plx:Right>
								<plx:Operator type="Add">
									<plx:Left>
										<plx:Value type="Local" data="{$iterVarName}"/>
									</plx:Left>
									<plx:Right>
										<plx:Value type="I4" data="1"/>
									</plx:Right>
								</plx:Operator>
							</plx:Right>
						</plx:Operator>
					</plx:LoopIncrement>
					<plx:Body>
						<!-- Initialize variables used for all styles of predicate replacement -->
						<plx:Variable name="currentRole" dataTypeName="Role">
							<plx:Initialize>
								<plx:CallInstance name="" type="ArrayIndexer">
									<plx:CallObject>
										<plx:Value type="Local" data="allRoles"/>
									</plx:CallObject>
									<plx:PassParam>
										<plx:Value type="Local" data="{$iterVarName}"/>
									</plx:PassParam>
								</plx:CallInstance>
							</plx:Initialize>
						</plx:Variable>
						<plx:Variable name="roleReplacement" dataTypeName="String" dataTypeQualifier="System">
							<plx:Initialize>
								<plx:NullObjectKeyword/>
							</plx:Initialize>
						</plx:Variable>
						<plx:Variable name="basicReplacement" dataTypeName="String" dataTypeQualifier="System">
							<plx:Initialize>
								<plx:CallInstance name="" type="ArrayIndexer">
									<plx:CallObject>
										<plx:Value type="Local" data="basicRoleReplacements"/>
									</plx:CallObject>
									<plx:PassParam>
										<plx:Value type="Local" data="{$iterVarName}"/>
									</plx:PassParam>
								</plx:CallInstance>
							</plx:Initialize>
						</plx:Variable>

						<!-- Do specialized replacement for different role matches -->
						<xsl:for-each select="ve:PredicateReplacement">
							<!-- The assumption is made here that predicate replacement quantifiers
								 are single-valued. -->
							<xsl:if test="position()=1">
								<plx:Condition>
									<plx:Test>
										<xsl:call-template name="PredicateReplacementConditionTest">
											<xsl:with-param name="Match" select="@match"/>
											<xsl:with-param name="IteratorContext" select="$IteratorContext"/>
										</xsl:call-template>
									</plx:Test>
									<plx:Body>
										<xsl:call-template name="PredicateReplacementBody"/>
									</plx:Body>
									<xsl:for-each select="following-sibling::*">
										<xsl:choose>
											<xsl:when test="(position()!=last()) or (string-length(@match) and not(@match='all'))">
												<plx:FallbackCondition>
													<plx:Test>
														<xsl:call-template name="PredicateReplacementConditionTest">
															<xsl:with-param name="Match" select="@match"/>
															<xsl:with-param name="IteratorContext" select="$IteratorContext"/>
														</xsl:call-template>
													</plx:Test>
													<plx:Body>
														<xsl:call-template name="PredicateReplacementBody"/>
													</plx:Body>
												</plx:FallbackCondition>
											</xsl:when>
											<xsl:otherwise>
												<plx:Alternate>
													<xsl:call-template name="PredicateReplacementBody"/>
												</plx:Alternate>
											</xsl:otherwise>
										</xsl:choose>
									</xsl:for-each>
								</plx:Condition>
							</xsl:if>
						</xsl:for-each>

						<!-- Use the default replacement for the predicate text if nothing was specified -->
						<plx:Condition>
							<plx:Test>
								<plx:Operator type="IdentityEquality">
									<plx:Left>
										<plx:Value type="Local" data="roleReplacement"/>
									</plx:Left>
									<plx:Right>
										<plx:NullObjectKeyword/>
									</plx:Right>
								</plx:Operator>
							</plx:Test>
							<plx:Body>
								<plx:Operator type="Assign">
									<plx:Left>
										<plx:Value type="Local" data="roleReplacement"/>
									</plx:Left>
									<plx:Right>
										<plx:Value type="Local" data="basicReplacement"/>
									</plx:Right>
								</plx:Operator>
							</plx:Body>
						</plx:Condition>
						<plx:Operator type="Assign">
							<plx:Left>
								<plx:CallInstance name="" type="ArrayIndexer">
									<plx:CallObject>
										<plx:Value type="Local" data="roleReplacements"/>
									</plx:CallObject>
									<plx:PassParam>
										<plx:Value type="Local" data="{$iterVarName}"/>
									</plx:PassParam>
								</plx:CallInstance>
							</plx:Left>
							<plx:Right>
								<plx:Value type="Local" data="roleReplacement"/>
							</plx:Right>
						</plx:Operator>
					</plx:Body>
				</plx:Loop>
				<!-- Append this fact to our temp statement builder-->
				<plx:CallInstance name="Append">
					<plx:CallObject>
						<plx:Value type="Local" data="sbTemp"/>
					</plx:CallObject>
					<plx:PassParam>
						<plx:CallStatic name="PopulatePredicateText" dataTypeName="FactType">
							<plx:PassParam>
								<plx:Value type="Local" data="readingOrder"/>
							</plx:PassParam>
							<plx:PassParam>
								<plx:Value type="Local" data="allRoles"/>
							</plx:PassParam>
							<plx:PassParam>
								<plx:Value type="Local" data="roleReplacements"/>
							</plx:PassParam>
						</plx:CallStatic>
					</plx:PassParam>
				</plx:CallInstance>
				<plx:Operator type="Assign">
					<plx:Left>
						<plx:Value type="Local" data="{$VariablePrefix}{$VariableDecorator}"/>
					</plx:Left>
					<plx:Right>
						<plx:CallInstance name="ToString">
							<plx:CallObject>
								<plx:Value type="Local" data="sbTemp"/>
							</plx:CallObject>
						</plx:CallInstance>
					</plx:Right>
				</plx:Operator>
			</xsl:when>
			<xsl:otherwise>
				<plx:Operator type="Assign">
					<plx:Left>
						<plx:Value type="Local" data="{$VariablePrefix}{$VariableDecorator}"/>
					</plx:Left>
					<plx:Right>
						<plx:CallStatic name="PopulatePredicateText" dataTypeName="FactType">
							<plx:PassParam>
								<plx:Value type="Local" data="readingOrder"/>
							</plx:PassParam>
							<plx:PassParam>
								<plx:Value type="Local" data="allRoles"/>
							</plx:PassParam>
							<plx:PassParam>
								<plx:Value type="Local" data="basicRoleReplacements"/>
							</plx:PassParam>
						</plx:CallStatic>
					</plx:Right>
				</plx:Operator>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template name="PredicateReplacementConditionTest">
		<xsl:param name="Match"/>
		<xsl:param name="IteratorContext"/>
		<xsl:choose>
			<xsl:when test="$Match='primary'">
				<plx:Operator type="IdentityEquality">
					<plx:Left>
						<plx:Value type="Local" data="primaryRole"/>
					</plx:Left>
					<plx:Right>
						<plx:Value type="Local" data="currentRole"/>
					</plx:Right>
				</plx:Operator>
			</xsl:when>
			<xsl:when test="$Match='secondary'">
				<xsl:choose>
					<xsl:when test="$IteratorContext='included'">
						<plx:Operator type="BooleanAnd">
							<plx:Left>
								<plx:Operator type="IdentityInequality">
									<plx:Left>
										<plx:Value type="Local" data="primaryRole"/>
									</plx:Left>
									<plx:Right>
										<plx:Value type="Local" data="currentRole"/>
									</plx:Right>
								</plx:Operator>
							</plx:Left>
							<plx:Right>
								<plx:Operator type="BooleanNot">
									<plx:CallInstance name="Contains">
										<plx:CallObject>
											<plx:Value type="Local" data="includedRoles"/>
										</plx:CallObject>
										<plx:PassParam>
											<plx:Value type="Local" data="currentRole"/>
										</plx:PassParam>
									</plx:CallInstance>
								</plx:Operator>
							</plx:Right>
						</plx:Operator>
					</xsl:when>
					<xsl:when test="$IteratorContext='excluded'">
						<plx:Operator type="BooleanAnd">
							<plx:Left>
								<plx:Operator type="IdentityInequality">
									<plx:Left>
										<plx:Value type="Local" data="primaryRole"/>
									</plx:Left>
									<plx:Right>
										<plx:Value type="Local" data="currentRole"/>
									</plx:Right>
								</plx:Operator>
							</plx:Left>
							<plx:Right>
								<plx:CallInstance name="Contains">
									<plx:CallObject>
										<plx:Value type="Local" data="includedRoles"/>
									</plx:CallObject>
									<plx:PassParam>
										<plx:Value type="Local" data="currentRole"/>
									</plx:PassParam>
								</plx:CallInstance>
							</plx:Right>
						</plx:Operator>
					</xsl:when>
					<xsl:otherwise>
						<plx:Operator type="IdentityInequality">
							<plx:Left>
								<plx:Value type="Local" data="primaryRole"/>
							</plx:Left>
							<plx:Right>
								<plx:Value type="Local" data="currentRole"/>
							</plx:Right>
						</plx:Operator>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<xsl:when test="$Match='included'">
				<plx:CallInstance name="Contains">
					<plx:CallObject>
						<plx:Value type="Local" data="includedRoles"/>
					</plx:CallObject>
					<plx:PassParam>
						<plx:Value type="Local" data="currentRole"/>
					</plx:PassParam>
				</plx:CallInstance>
			</xsl:when>
			<xsl:when test="$Match='excluded'">
				<plx:Operator type="BooleanNot">
					<plx:CallInstance name="Contains">
						<plx:CallObject>
							<plx:Value type="Local" data="includedRoles"/>
						</plx:CallObject>
						<plx:PassParam>
							<plx:Value type="Local" data="currentRole"/>
						</plx:PassParam>
					</plx:CallInstance>
				</plx:Operator>
			</xsl:when>
		</xsl:choose>
	</xsl:template>
	<xsl:template name="PredicateReplacementBody">
		<xsl:for-each select="ve:Quantifier">
			<plx:Operator type="Assign">
				<plx:Left>
					<plx:Value type="Local" data="roleReplacement"/>
				</plx:Left>
				<plx:Right>
					<plx:CallStatic name="Format" dataTypeName="String" dataTypeQualifier="System">
						<plx:PassParam>
							<plx:CallStatic name="CurrentUICulture" dataTypeName="CultureInfo" type="Property"/>
						</plx:PassParam>
						<plx:PassParam>
							<xsl:call-template name="SnippetFor">
								<xsl:with-param name="SnippetType" select="@type"/>
							</xsl:call-template>
						</plx:PassParam>
						<plx:PassParam>
							<plx:Value type="Local" data="basicReplacement"/>
						</plx:PassParam>
					</plx:CallStatic>
				</plx:Right>
			</plx:Operator>
		</xsl:for-each>
	</xsl:template>
	<xsl:template match="ve:IterateRoles" mode="InternalConstraintVerbalization">
		<xsl:param name="VariableDecorator" select="position()"/>
		<xsl:param name="VariablePrefix" select="'factText'"/>
		<xsl:variable name="contextMatchFragment">
			<xsl:choose>
				<xsl:when test="string-length(@match)">
					<xsl:value-of select="@match"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:text>all</xsl:text>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:variable name="contextMatch" select="string($contextMatchFragment)"/>
		<xsl:variable name="iterVarName" select="concat($VariablePrefix,'roleIter',$VariableDecorator)"/>
		<xsl:call-template name="EnsureTempStringBuilder"/>
		<plx:Variable name="{$iterVarName}" dataTypeName="Int32" dataTypeQualifier="System">
			<plx:Initialize>
				<plx:Value type="I4" data="0"/>
			</plx:Initialize>
		</plx:Variable>
		<plx:Loop>
			<plx:LoopTest apply="Before">
				<plx:Operator type="LessThan">
					<plx:Left>
						<plx:Value type="Local" data="{$iterVarName}"/>
					</plx:Left>
					<plx:Right>
						<plx:Value type="Local">
							<xsl:attribute name="data">
								<xsl:choose>
									<xsl:when test="$contextMatch='all'">
										<xsl:text>fullArity</xsl:text>
									</xsl:when>
									<xsl:when test="$contextMatch='included'">
										<xsl:text>includedArity</xsl:text>
									</xsl:when>
									<!-- UNDONE: Support excluded match -->
								</xsl:choose>
							</xsl:attribute>
						</plx:Value>
					</plx:Right>
				</plx:Operator>
			</plx:LoopTest>
			<plx:LoopIncrement>
				<plx:Operator type="Assign">
					<plx:Left>
						<plx:Value type="Local" data="{$iterVarName}"/>
					</plx:Left>
					<plx:Right>
						<plx:Operator type="Add">
							<plx:Left>
								<plx:Value type="Local" data="{$iterVarName}"/>
							</plx:Left>
							<plx:Right>
								<plx:Value type="I4" data="1"/>
							</plx:Right>
						</plx:Operator>
					</plx:Right>
				</plx:Operator>
			</plx:LoopIncrement>
			<plx:Body>
				<plx:Variable name="primaryRole" dataTypeName="Role">
					<plx:Initialize>
						<plx:CallInstance name="" type="ArrayIndexer">
							<plx:CallObject>
								<plx:Value type="Local">
									<xsl:attribute name="data">
										<xsl:choose>
											<xsl:when test="$contextMatch='all'">
												<xsl:text>allRoles</xsl:text>
											</xsl:when>
											<xsl:when test="$contextMatch='included'">
												<xsl:text>includedRoles</xsl:text>
											</xsl:when>
											<!-- UNDONE: Support excluded match -->
										</xsl:choose>
									</xsl:attribute>
								</plx:Value>
							</plx:CallObject>
							<plx:PassParam>
								<plx:Value type="Local" data="{$iterVarName}"/>
							</plx:PassParam>
						</plx:CallInstance>
					</plx:Initialize>
				</plx:Variable>
				<!-- Use the current snippets data to open the list -->
				<plx:Condition>
					<plx:Test>
						<plx:Operator type="Equality">
							<plx:Left>
								<plx:Value type="Local" data="{$iterVarName}"/>
							</plx:Left>
							<plx:Right>
								<plx:Value type="I4" data="0"/>
							</plx:Right>
						</plx:Operator>
					</plx:Test>
					<plx:Body>
						<plx:CallInstance name="Append">
							<plx:CallObject>
								<plx:Value type="Local" data="sbTemp"/>
							</plx:CallObject>
							<plx:PassParam>
								<xsl:call-template name="SnippetFor">
									<xsl:with-param name="SnippetType" select="concat(@listStyle,'Open')"/>
								</xsl:call-template>
							</plx:PassParam>
						</plx:CallInstance>
					</plx:Body>
					<plx:FallbackCondition>
						<plx:Test>
							<plx:Operator type="Equality">
								<plx:Left>
									<plx:Value type="Local" data="{$iterVarName}"/>
								</plx:Left>
								<plx:Right>
									<plx:Operator type="Subtract">
										<plx:Left>
											<plx:Value type="Local">
												<xsl:attribute name="data">
													<xsl:choose>
														<xsl:when test="$contextMatch='all'">
															<xsl:text>fullArity</xsl:text>
														</xsl:when>
														<xsl:when test="$contextMatch='included'">
															<xsl:text>includedArity</xsl:text>
														</xsl:when>
														<!-- UNDONE: Support excluded match -->
													</xsl:choose>
												</xsl:attribute>
											</plx:Value>
										</plx:Left>
										<plx:Right>
											<plx:Value type="I4" data="1"/>
										</plx:Right>
									</plx:Operator>
								</plx:Right>
							</plx:Operator>
						</plx:Test>
						<plx:Body>
							<plx:CallInstance name="Append">
								<plx:CallObject>
									<plx:Value type="Local" data="sbTemp"/>
								</plx:CallObject>
								<plx:PassParam>
									<xsl:call-template name="SnippetFor">
										<xsl:with-param name="SnippetType" select="concat(@listStyle,'FinalSeparator')"/>
									</xsl:call-template>
								</plx:PassParam>
							</plx:CallInstance>
						</plx:Body>
					</plx:FallbackCondition>
					<plx:Alternate>
						<plx:CallInstance name="Append">
							<plx:CallObject>
								<plx:Value type="Local" data="sbTemp"/>
							</plx:CallObject>
							<plx:PassParam>
								<xsl:call-template name="SnippetFor">
									<xsl:with-param name="SnippetType" select="concat(@listStyle,'FinalSeparator')"/>
								</xsl:call-template>
							</plx:PassParam>
						</plx:CallInstance>
					</plx:Alternate>
				</plx:Condition>

				<!-- Process the child contents for this role -->
				<xsl:for-each select="child::ve:*">
					<xsl:apply-templates select="." mode="InternalConstraintVerbalization">
						<xsl:with-param name="VariablePrefix" select="$VariablePrefix"/>
						<xsl:with-param name="IteratorContext" select="$contextMatch"/>
					</xsl:apply-templates>
				</xsl:for-each>
				
				<!-- Use the current snippets data to close the list -->
				<plx:Condition>
					<plx:Test>
						<plx:Operator type="Equality">
							<plx:Left>
								<plx:Value type="Local" data="{$iterVarName}"/>
							</plx:Left>
							<plx:Right>
								<plx:Operator type="Subtract">
									<plx:Left>
										<plx:Value type="Local">
											<xsl:attribute name="data">
												<xsl:choose>
													<xsl:when test="$contextMatch='all'">
														<xsl:text>fullArity</xsl:text>
													</xsl:when>
													<xsl:when test="$contextMatch='included'">
														<xsl:text>includedArity</xsl:text>
													</xsl:when>
													<!-- UNDONE: Support excluded match -->
												</xsl:choose>
											</xsl:attribute>
										</plx:Value>
									</plx:Left>
									<plx:Right>
										<plx:Value type="I4" data="1"/>
									</plx:Right>
								</plx:Operator>
							</plx:Right>
						</plx:Operator>
					</plx:Test>
					<plx:Body>
						<plx:CallInstance name="Append">
							<plx:CallObject>
								<plx:Value type="Local" data="sbTemp"/>
							</plx:CallObject>
							<plx:PassParam>
								<xsl:call-template name="SnippetFor">
									<xsl:with-param name="SnippetType" select="concat(@listStyle,'Close')"/>
								</xsl:call-template>
							</plx:PassParam>
						</plx:CallInstance>
					</plx:Body>
				</plx:Condition>
			</plx:Body>
		</plx:Loop>
	</xsl:template>
	<!-- Get the snippet value from the current snippets set.
		 This assumes snippets, isDeontic and isNegative local
		 variables are defined -->
	<xsl:template name="SnippetFor">
		<xsl:param name="SnippetType"/>
		<plx:CallInstance name="GetSnippet">
			<plx:CallObject>
				<plx:Value type="Local" data="snippets"/>
			</plx:CallObject>
			<plx:PassParam>
				<plx:CallStatic name="{$SnippetType}" dataTypeName="{$VerbalizationTextSnippetType}" type="Field"/>
			</plx:PassParam>
			<plx:PassParam>
				<plx:Value type="Local" data="isDeontic"/>
			</plx:PassParam>
			<plx:PassParam>
				<plx:Value type="Local" data="isNegative"/>
			</plx:PassParam>
		</plx:CallInstance>
	</xsl:template>
	<!-- Helper function to create an initialized string builder in the sbTemp local variable -->
	<xsl:template name="EnsureTempStringBuilder">
		<plx:Condition>
			<plx:Test>
				<plx:Operator type="IdentityEquality">
					<plx:Left>
						<plx:Value type="Local" data="sbTemp"/>
					</plx:Left>
					<plx:Right>
						<plx:NullObjectKeyword/>
					</plx:Right>
				</plx:Operator>
			</plx:Test>
			<plx:Body>
				<plx:Operator type="Assign">
					<plx:Left>
						<plx:Value type="Local" data="sbTemp"/>
					</plx:Left>
					<plx:Right>
						<plx:CallNew dataTypeName="StringBuilder"/>
					</plx:Right>
				</plx:Operator>
			</plx:Body>
			<plx:Alternate>
				<plx:Operator type="Assign">
					<plx:Left>
						<plx:CallInstance name="Length" type="Property">
							<plx:CallObject>
								<plx:Value type="Local" data="sbTemp"/>
							</plx:CallObject>
						</plx:CallInstance>
					</plx:Left>
					<plx:Right>
						<plx:Value type="I4" data="0"/>
					</plx:Right>
				</plx:Operator>
			</plx:Alternate>
		</plx:Condition>
	</xsl:template>
	<xsl:template name="PopulateReadingOrder">
		<!-- Support readings for PreferPrimaryLeadReading, RequirePrimaryLeadReading,
			 RequireLeadReading, PreferLeadReading, and empty match properties -->
		<xsl:param name="readingChoice"/>
		<xsl:if test="not($readingChoice='Conditional')">
			<plx:Operator type="Assign">
				<plx:Left>
					<plx:Value type="Local" data="readingOrder"/>
				</plx:Left>
				<plx:Right>
					<plx:CallStatic name="GetMatchingReadingOrder" dataTypeName="FactType">
						<plx:PassParam>
							<!-- The readingOrders param-->
							<plx:Value type="Local" data="allReadingOrders"/>
						</plx:PassParam>
						<plx:PassParam>
							<!-- The matchLeadRole param -->
							<xsl:choose>
								<xsl:when test="contains($readingChoice,'PrimaryLeadReading')">
									<plx:Value type="Local" data="primaryRole"/>
								</xsl:when>
								<xsl:when test="contains($readingChoice, 'LeadReading')">
									<plx:NullObjectKeyword/>
								</xsl:when>
								<xsl:otherwise>
									<plx:CallInstance name="" type="ArrayIndexer">
										<plx:CallObject>
											<plx:Value type="Local" data="allRoles"/>
										</plx:CallObject>
										<plx:PassParam>
											<plx:Value type="I4" data="0"/>
										</plx:PassParam>
									</plx:CallInstance>
								</xsl:otherwise>
							</xsl:choose>
						</plx:PassParam>
						<plx:PassParam>
							<!-- The matchAnyLeadRole param -->
							<xsl:choose>
								<xsl:when test="contains($readingChoice,'LeadReading') and not(contains($readingChoice,'PrimaryLeadReading'))">
									<!-- UNDONE: Support excluded roles as weill -->
									<plx:Value type="Local" data="includedRoles"/>
								</xsl:when>
								<xsl:otherwise>
									<plx:NullObjectKeyword/>
								</xsl:otherwise>
							</xsl:choose>
						</plx:PassParam>
						<plx:PassParam>
							<!-- The defaultRoleOrder param -->
							<plx:Value type="Local" data="allRoles"/>
						</plx:PassParam>
						<plx:PassParam>
							<!-- The allowAnyOrder param -->
							<xsl:choose>
								<xsl:when test="not(starts-with($readingChoice,'Require'))">
									<plx:TrueKeyword/>
								</xsl:when>
								<xsl:otherwise>
									<plx:FalseKeyword/>
								</xsl:otherwise>
							</xsl:choose>
						</plx:PassParam>
					</plx:CallStatic>
				</plx:Right>
			</plx:Operator>
		</xsl:if>
	</xsl:template>
</xsl:stylesheet>